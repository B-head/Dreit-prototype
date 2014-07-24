using AbstractSyntax;
using AbstractSyntax.Directive;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    public class CilImport
    {
        private Root Root;
        public Dictionary<object, Element> ImportDictionary { get; private set; }

        private CilImport(Root root)
        {
            Root = root;
            ImportDictionary = new Dictionary<object, Element>();
        }

        public static CilImport Import(Root root, Assembly assembly)
        {
            var ret = new CilImport(root);
            ret.ImportAssembly(assembly);
            return ret;
        }

        private NameSpaceSymbol GetNameSpace(string name)
        {
            var nl = name.Split('.');
            NameSpaceSymbol ret = Root;
            foreach(var v in nl)
            {
                var temp = (NameSpaceSymbol)ret.FindName(v);
                if(temp == null)
                {
                    temp = new NameSpaceSymbol(v);
                    ret.Append(temp);
                }
                ret = temp;
            }
            return ret;
        }

        private void ImportAssembly(Assembly assembly)
        {
            var module = assembly.GetModules();
            foreach (var m in module)
            {
                ImportModule(m);
            }
        }

        private void ImportModule(Module module)
        {
            var type = module.GetTypes();
            foreach (var t in type)
            {
                if (!t.IsPublic)
                {
                    continue;
                }
                var ns = GetNameSpace(t.Namespace);
                if (t.IsEnum)
                {
                    ns.Append(ImportEnum(t));
                }
                else
                {
                    ns.Append(ImportType(t));
                }
            }
        }

        private Scope ImportType(Type type)
        {
            if (type.IsEnum)
            {
                return ImportEnum(type);
            }
            if (type.IsGenericType)
            {
                return ImportGenericType(type);
            }
            if (type.IsGenericTypeDefinition || !(type.IsArray || type.IsByRef || type.IsPointer))
            {
                return ImportPureType(type);
            }
            return ImportQualifyType(type);
        }

        private ClassSymbol ImportPureType(Type type)
        {
            bool isTrait;
            var attribute = CreateAttributeList(type.Attributes, out isTrait);
            var generic = CreateGenericList(type.GetGenericArguments());
            var inherit = CreateInheritList(type);
            var exps = new List<Element>();
            var ctor = type.GetConstructors();
            foreach (var c in ctor)
            {
                exps.Add(ImportConstructor(c));
            }
            //todo Eventのインポートに対応する。
            //var eve = type.GetEvents();
            //foreach (var e in eve)
            //{
            //    exp.Add(ImportEvent(e));
            //}
            var property = type.GetProperties();
            foreach (var p in property)
            {
                if (p.GetMethod != null)
                {
                    exps.Add(ImportProperty(p.GetMethod, p.Name));
                }
                if (p.SetMethod != null)
                {
                    exps.Add(ImportProperty(p.SetMethod, p.Name));
                }
            }
            var method = type.GetMethods();
            foreach (var m in method)
            {
                exps.Add(ImportMethod(m));
            }
            var field = type.GetFields();
            foreach (var f in field)
            {
                exps.Add(ImportField(f));
            }
            var nested = type.GetNestedTypes();
            foreach (var n in nested)
            {
                exps.Add(ImportType(n));
            }
            var block = new DirectiveList(exps);
            var elem = new ClassSymbol(type.Name, isTrait, block, attribute, generic, inherit);
            ImportDictionary.Add(type, elem);
            return elem;
        }

        private QualifyTypeSymbol ImportQualifyType(Type type)
        {
            return null;
        }

        private GenericSymbol ImportGenericType(Type type)
        {
            var attribute = CreateAttributeList(type.GenericParameterAttributes);
            var constraint = CreateConstraintList(type.GetGenericParameterConstraints());
            var elem = new GenericSymbol(type.Name, attribute, constraint);
            ImportDictionary.Add(type, elem);
            return elem;
        }

        private EnumSymbol ImportEnum(Type type)
        {
            bool isTrait;
            var attribute = CreateAttributeList(type.Attributes, out isTrait);
            var dt = ImportType(type.GetEnumUnderlyingType());
            var exps = new List<Element>();
            foreach(var v in type.GetEnumNames())
            {
                var f = new VariantSymbol(v, true, new List<Scope>(), dt);
                exps.Add(f);
            }
            var block = new DirectiveList(exps);
            var elem = new EnumSymbol(type.Name, block, attribute, dt);
            ImportDictionary.Add(type, elem);
            return elem;

        }

        private RoutineSymbol ImportMethod(MethodInfo method)
        {
            var attribute = CreateAttributeList(method.Attributes);
            var generic = CreateGenericList(method.GetGenericArguments());
            var arguments = CreateArgumentList(method);
            var rt = ImportType(method.ReturnType);
            var elem = new RoutineSymbol(method.Name, TokenType.Unknoun, attribute, generic, arguments, rt);
            ImportDictionary.Add(method, elem);
            return elem;
        }

        private RoutineSymbol ImportProperty(MethodInfo prop, string name)
        {
            var attribute = CreateAttributeList(prop.Attributes);
            var generic = new List<GenericSymbol>();
            var arguments = CreateArgumentList(prop);
            var rt = ImportType(prop.ReturnType);
            var elem = new RoutineSymbol(name, TokenType.Unknoun, attribute, generic, arguments, rt);
            ImportDictionary.Add(prop, elem);
            return elem;
        }

        private RoutineSymbol ImportConstructor(ConstructorInfo ctor)
        {
            var attribute = CreateAttributeList(ctor.Attributes);
            var generic = new List<GenericSymbol>();
            var arguments = CreateArgumentList(ctor);
            var rt = ImportType(ctor.DeclaringType);
            var elem = new RoutineSymbol(RoutineSymbol.ConstructorIdentifier, TokenType.Unknoun, attribute, generic, arguments, rt);
            ImportDictionary.Add(ctor, elem);
            return elem;
        }

        private ArgumentSymbol ImportArgument(ParameterInfo prm)
        {
            var attribute = CreateAttributeList(prm.Attributes);
            var dt = ImportType(prm.ParameterType);
            var elem = new ArgumentSymbol(prm.Name, attribute, dt);
            ImportDictionary.Add(prm, elem);
            return elem;
        }

        private VariantSymbol ImportField(FieldInfo field)
        {
            bool isLet = false;
            var attribute = CreateAttributeList(field.Attributes, out isLet);
            var dt = ImportType(field.FieldType);
            var elem = new VariantSymbol(field.Name, isLet, attribute, dt);
            ImportDictionary.Add(field, elem);
            return elem;
        }

        private IReadOnlyList<Scope> CreateAttributeList(TypeAttributes attr, out bool isTrait)
        {
            isTrait = false;
            var ret = new List<Scope>();
            if (attr.HasFlag(TypeAttributes.Abstract)) ret.Add(Root.Abstract);
            if (attr.HasFlag(TypeAttributes.Class)) isTrait = false;
            if (attr.HasFlag(TypeAttributes.Interface)) isTrait = true;
            if (attr.HasFlag(TypeAttributes.NestedFamily)) ret.Add(Root.Protected);
            if (attr.HasFlag(TypeAttributes.NestedFamORAssem)) ret.Add(Root.Protected);
            if (attr.HasFlag(TypeAttributes.NestedPublic)) ret.Add(Root.Public);
            if (attr.HasFlag(TypeAttributes.Public)) ret.Add(Root.Public);
            return ret;
        }

        private IReadOnlyList<Scope> CreateAttributeList(GenericParameterAttributes attr)
        {
            var ret = new List<Scope>();
            if (attr.HasFlag(GenericParameterAttributes.Contravariant)) ret.Add(Root.Contravariant);
            if (attr.HasFlag(GenericParameterAttributes.Covariant)) ret.Add(Root.Covariant);
            if (attr.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint)) ret.Add(Root.ConstructorConstraint);
            if (attr.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) ret.Add(Root.ValueConstraint);
            if (attr.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) ret.Add(Root.ReferenceConstraint);
            return ret;
        }

        private IReadOnlyList<Scope> CreateAttributeList(MethodAttributes attr)
        {
            var ret = new List<Scope>();
            if (attr.HasFlag(MethodAttributes.Abstract)) ret.Add(Root.Abstract);
            if (attr.HasFlag(MethodAttributes.Family)) ret.Add(Root.Protected);
            if (attr.HasFlag(MethodAttributes.FamORAssem)) ret.Add(Root.Protected);
            if (attr.HasFlag(MethodAttributes.Final)) ret.Add(Root.Final);
            if (attr.HasFlag(MethodAttributes.Public)) ret.Add(Root.Public);
            if (attr.HasFlag(MethodAttributes.Static)) ret.Add(Root.Static);
            if (attr.HasFlag(MethodAttributes.Virtual)) ret.Add(Root.Virtual);
            return ret;
        }

        private IReadOnlyList<Scope> CreateAttributeList(FieldAttributes attr, out bool isLet)
        {
            isLet = false;
            var ret = new List<Scope>();
            if (attr.HasFlag(FieldAttributes.Family)) ret.Add(Root.Protected);
            if (attr.HasFlag(FieldAttributes.FamORAssem)) ret.Add(Root.Protected);
            if (attr.HasFlag(FieldAttributes.InitOnly)) isLet = true;
            if (attr.HasFlag(FieldAttributes.Public)) ret.Add(Root.Public);
            if (attr.HasFlag(FieldAttributes.Static)) ret.Add(Root.Static);
            return ret;
        }

        private IReadOnlyList<Scope> CreateAttributeList(ParameterAttributes attr)
        {
            var ret = new List<Scope>();
            if (attr.HasFlag(ParameterAttributes.HasDefault)) ret.Add(Root.Optional);
            if (attr.HasFlag(ParameterAttributes.Optional)) ret.Add(Root.Optional);
            return ret;
        }

        private IReadOnlyList<GenericSymbol> CreateGenericList(Type[] gnr)
        {
            var ret = new List<GenericSymbol>();
            foreach(var v in gnr)
            {
                ret.Add(ImportGenericType(v));
            }
            return ret;
        }

        private IReadOnlyList<Scope> CreateInheritList(Type type)
        {
            var ret = new List<Scope>();
            ret.Add(ImportType(type.BaseType));
            foreach(var v in type.GetInterfaces())
            {
                ret.Add(ImportType(v));
            }
            return ret;
        }

        private IReadOnlyList<Scope> CreateConstraintList(Type[] types)
        {
            var ret = new List<Scope>();
            foreach (var v in types)
            {
                ret.Add(ImportType(v));
            }
            return ret;
        }

        private IReadOnlyList<ArgumentSymbol> CreateArgumentList(MethodBase method)
        {
            var ret = new List<ArgumentSymbol>();
            foreach (var v in method.GetParameters())
            {
                ret.Add(ImportArgument(v));
            }
            return ret;
        }
    }
}
