using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using CoreLibrary;
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
        public Dictionary<object, Scope> ImportDictionary { get; private set; }
        private const BindingFlags Binding = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        public CilImport(Root root)
        {
            Root = root;
            ImportDictionary = new Dictionary<object, Scope>();
            ImportDictionary.Add(typeof(void), Root.Void);
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

        public void ImportAssembly(Assembly assembly)
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
                if (typeof(void) == t)
                {
                    continue;
                }
                ns.Append(ImportType(t));
            }
        }

        private TypeSymbol ImportType(Type type)
        {
            if (type.HasElementType)
            {
                return ImportModifyType(type);
            }
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return ImportTemplateInstance(type);
            }
            if (type.IsGenericParameter)
            {
                return ImportGenericType(type);
            }
            if (type.IsEnum)
            {
                return ImportEnum(type);
            }
            return ImportPureType(type);
        }

        private ClassSymbol ImportPureType(Type type)
        {
            var elem = new ClassSymbol();
            if (ImportDictionary.ContainsKey(type))
            {
                return (ClassSymbol)ImportDictionary[type];
            }
            ImportDictionary.Add(type, elem);
            ClassType classType;
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, type, out classType);
            var generic = CreateGenericList(type.GetGenericArguments());
            var inherit = CreateInheritList(type);
            var block = new ProgramContext();
            var ctor = type.GetConstructors(Binding);
            foreach (var c in ctor)
            {
                if(c.IsAssembly || c.IsFamilyAndAssembly || c.IsPrivate)
                {
                    continue;
                }
                block.Append(ImportConstructor(c));
            }
            //todo Eventのインポートに対応する。
            //var eve = type.GetEvents();
            //foreach (var e in eve)
            //{
            //    block.Append(ImportEvent(e));
            //}
            var property = type.GetProperties(Binding);
            foreach (var p in property)
            {
                var name = p.GetIndexParameters().Count() > 0 ? RoutineSymbol.AliasCallIdentifier : p.Name; 
                var g = p.GetMethod;
                if (g != null && !g.IsAssembly && !g.IsFamilyAndAssembly && !g.IsPrivate)
                {
                    block.Append(ImportProperty(p.GetMethod, name));
                }
                var s = p.SetMethod;
                if (s != null && !s.IsAssembly && !s.IsFamilyAndAssembly && !s.IsPrivate)
                {
                    block.Append(ImportProperty(p.SetMethod, name));
                }
            }
            var method = type.GetMethods(Binding);
            foreach (var m in method)
            {
                if (m.IsAssembly || m.IsFamilyAndAssembly || m.IsPrivate)
                {
                    continue;
                }
                if(ImportDictionary.ContainsKey(m))
                {
                    continue;
                }
                block.Append(ImportMethod(m));
            }
            var field = type.GetFields(Binding);
            foreach (var f in field)
            {
                if (f.IsAssembly || f.IsFamilyAndAssembly || f.IsPrivate)
                {
                    continue;
                }
                block.Append(ImportField(f));
            }
            var nested = type.GetNestedTypes(Binding);
            foreach (var n in nested)
            {
                if (n.IsNestedAssembly || n.IsNestedFamANDAssem || n.IsNestedPrivate)
                {
                    continue;
                }
                block.Append(ImportType(n));
            }
            elem.Initialize(TrimTypeNameMangling(type.Name), classType, block, attribute, generic, inherit);
            return elem;
        }

        private ClassTemplateInstance ImportModifyType(Type type)
        {
            var elementType = ImportType(type.GetElementType());
            ClassTemplateInstance elem = null;
            if(type.IsArray)
            {
                elem = Root.ClassManager.Issue(Root.EmbedArray, new TypeSymbol[] { elementType });
            }
            else if(type.IsByRef)
            {
                elem = Root.ClassManager.Issue(Root.Refer, new TypeSymbol[] { elementType });
            }
            else if(type.IsPointer)
            {
                elem = Root.ClassManager.Issue(Root.Pointer, new TypeSymbol[] { elementType });
            }
            else
            {
                throw new ArgumentException("type");
            }
            if (ImportDictionary.ContainsKey(type))
            {
                return (ClassTemplateInstance)ImportDictionary[type];
            }
            ImportDictionary.Add(type, elem);
            return elem;
        }

        private ClassTemplateInstance ImportTemplateInstance(Type type)
        {
            var definition = ImportType(type.GetGenericTypeDefinition());
            var parameter = new List<TypeSymbol>();
            var elem = Root.ClassManager.Issue(definition, parameter);
            if (ImportDictionary.ContainsKey(type))
            {
                return (ClassTemplateInstance)ImportDictionary[type];
            }
            ImportDictionary.Add(type, elem);
            AppendParameterType(parameter, type.GetGenericArguments());
            return elem;
        }

        private GenericSymbol ImportGenericType(Type type)
        {
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, type);
            var constraint = CreateConstraintList(type.GetGenericParameterConstraints());
            var elem = new GenericSymbol(type.Name, attribute, constraint);
            if (ImportDictionary.ContainsKey(type))
            {
                return (GenericSymbol)ImportDictionary[type];
            }
            ImportDictionary.Add(type, elem);
            return elem;
        }

        private EnumSymbol ImportEnum(Type type)
        {
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, type);
            var dt = ImportType(type.GetEnumUnderlyingType());
            var block = new ProgramContext();
            var elem = new EnumSymbol(type.Name, block, attribute, dt);
            if (ImportDictionary.ContainsKey(type))
            {
                return (EnumSymbol)ImportDictionary[type];
            }
            ImportDictionary.Add(type, elem);
            foreach(var v in type.GetEnumNames())
            {
                var f = new VariantSymbol();
                f.Initialize(v, VariantType.Const, new List<AttributeSymbol>(), dt);
                block.Append(f);
            }
            return elem;
        }

        private RoutineSymbol ImportMethod(MethodInfo method)
        {
            var elem = new RoutineSymbol();
            if (ImportDictionary.ContainsKey(method))
            {
                return (RoutineSymbol)ImportDictionary[method];
            }
            ImportDictionary.Add(method, elem);
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, method);
            var generic = CreateGenericList(method.GetGenericArguments());
            var arguments = CreateArgumentList(method);
            var rt = ImportType(method.ReturnType);
            elem.Initialize(method.Name, RoutineType.Routine, TokenType.Unknoun, attribute, generic, arguments, rt);
            return elem;
        }

        private RoutineSymbol ImportProperty(MethodInfo prop, string name)
        {
            var elem = new RoutineSymbol();
            if (ImportDictionary.ContainsKey(prop))
            {
                return (RoutineSymbol)ImportDictionary[prop];
            }
            ImportDictionary.Add(prop, elem);
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, prop);
            var generic = new List<GenericSymbol>();
            var arguments = CreateArgumentList(prop);
            var rt = ImportType(prop.ReturnType);
            elem.Initialize(name, RoutineType.Routine, TokenType.Unknoun, attribute, generic, arguments, rt);
            return elem;
        }

        private RoutineSymbol ImportConstructor(ConstructorInfo ctor)
        {
            var elem = new RoutineSymbol();
            if (ImportDictionary.ContainsKey(ctor))
            {
                return (RoutineSymbol)ImportDictionary[ctor];
            }
            ImportDictionary.Add(ctor, elem);
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, ctor);
            var generic = new List<GenericSymbol>();
            var arguments = CreateArgumentList(ctor);
            var rt = ImportType(ctor.DeclaringType);
            elem.Initialize(RoutineSymbol.ConstructorIdentifier, RoutineType.Routine, TokenType.Unknoun, attribute, generic, arguments, rt);
            return elem;
        }

        private ParameterSymbol ImportArgument(ParameterInfo prm)
        {
            var elem = new ParameterSymbol();
            if (ImportDictionary.ContainsKey(prm))
            {
                return (ParameterSymbol)ImportDictionary[prm];
            }
            ImportDictionary.Add(prm, elem);
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, prm);
            var dt = ImportType(prm.ParameterType);
            elem.Initialize(prm.Name, VariantType.Var, attribute, dt);
            return elem;
        }

        private VariantSymbol ImportField(FieldInfo field)
        {
            var elem = new VariantSymbol();
            if (ImportDictionary.ContainsKey(field))
            {
                return (VariantSymbol)ImportDictionary[field];
            }
            ImportDictionary.Add(field, elem);
            VariantType type;
            var attribute = new List<AttributeSymbol>();
            AppendEmbededAttribute(attribute, field, out type);
            var dt = ImportType(field.FieldType);
            elem.Initialize(field.Name, type, attribute, dt);
            return elem;
        }

        private string TrimTypeNameMangling(string name)
        {
            var i = name.IndexOf('`');
            if(i < 0)
            {
                return name;
            }
            return name.Substring(0, i);
        }

        private void AppendEmbededAttribute(List<AttributeSymbol> list, Type type)
        {
            ClassType classType;
            AppendEmbededAttribute(list, type, out classType);
        }

        private void AppendEmbededAttribute(List<AttributeSymbol> list, Type type, out ClassType classType)
        {
            classType = ClassType.Class;
            if (type.GetCustomAttribute<GlobalScopeAttribute>() != null) list.Add(Root.GlobalScope);
            if (type.IsAbstract) list.Add(Root.Abstract);
            if (type.IsClass) classType = ClassType.Class;
            if (type.IsInterface) classType = ClassType.Trait;
            if (type.IsNestedFamily) list.Add(Root.Protected);
            if (type.IsNestedFamORAssem) list.Add(Root.Protected);
            if (type.IsNestedPublic) list.Add(Root.Public);
            if (type.IsPublic) list.Add(Root.Public);
            if (type.IsSealed) list.Add(Root.Final);
            if (type.IsValueType) classType = ClassType.Class;
            if (type.IsGenericParameter)
            {
                var gattr = type.GenericParameterAttributes;
                if (gattr.HasFlag(GenericParameterAttributes.Contravariant)) list.Add(Root.Contravariant);
                if (gattr.HasFlag(GenericParameterAttributes.Covariant)) list.Add(Root.Covariant);
                if (gattr.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint)) list.Add(Root.ConstructorConstraint);
                if (gattr.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) list.Add(Root.ValueConstraint);
                if (gattr.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) list.Add(Root.ReferenceConstraint);
            }
        }

        private void AppendEmbededAttribute(List<AttributeSymbol> list, MethodBase method)
        {
            if (method.IsAbstract) list.Add(Root.Abstract);
            if (method.IsFamily) list.Add(Root.Protected);
            if (method.IsFamilyOrAssembly) list.Add(Root.Protected);
            if (method.IsFinal) list.Add(Root.Final);
            if (method.IsPublic) list.Add(Root.Public);
            if (method.IsStatic) list.Add(Root.Static);
            if (method.IsVirtual) list.Add(Root.Virtual);
        }

        private void AppendEmbededAttribute(List<AttributeSymbol> list, FieldInfo field, out VariantType type)
        {
            type = VariantType.Var;
            if (field.IsFamily) list.Add(Root.Protected);
            if (field.IsFamilyOrAssembly) list.Add(Root.Protected);
            if (field.IsInitOnly) type = VariantType.Let;
            if (field.IsLiteral) type = VariantType.Const;
            if (field.IsPublic) list.Add(Root.Public);
            if (field.IsStatic) list.Add(Root.Static);
        }

        private void AppendEmbededAttribute(List<AttributeSymbol> list, ParameterInfo parameter)
        {
            if (parameter.GetCustomAttribute<ParamArrayAttribute>() != null) list.Add(Root.Variadic);
            if (parameter.IsOptional) list.Add(Root.Optional);
        }

        private void AppendParameterType(List<TypeSymbol> list, Type[] prm)
        {
            foreach (var v in prm)
            {
                list.Add(ImportType(v));
            }
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

        private IReadOnlyList<TypeSymbol> CreateInheritList(Type type)
        {
            var ret = new List<TypeSymbol>();
            if (type.BaseType != null)
            {
                ret.Add(ImportType(type.BaseType));
            }
            foreach(var v in type.GetInterfaces())
            {
                if (!v.IsPublic && !v.IsNestedPublic && !v.IsNestedFamily && !v.IsNestedFamORAssem)
                {
                    continue;
                }
                var t = ImportType(v);
                if (t != null)
                {
                    ret.Add(t);
                }
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

        private IReadOnlyList<ParameterSymbol> CreateArgumentList(MethodBase method)
        {
            var ret = new List<ParameterSymbol>();
            foreach (var v in method.GetParameters())
            {
                ret.Add(ImportArgument(v));
            }
            return ret;
        }
    }
}
