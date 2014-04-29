using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;

namespace CliTranslate
{
    public class ImportManager
    {
        private Root Root;
        private List<InfoPeir> Peir;
        private ImportNameSpace NameSpace;

        public ImportManager(Root root)
        {
            Root = root;
            Peir = new List<InfoPeir>();
            NameSpace = new ImportNameSpace { NameSpace = Root };
        }

        public void ImportAssembly(Assembly assembly)
        {
            var module = assembly.GetModules();
            foreach(var m in module)
            {
                ImportModule(m);
            }
        }

        public void TranslateImport(RootTranslator root)
        {
            foreach(var v in Peir)
            {
                root.RegisterBuilder(v.Scope, v.Info);
            }
        }

        private void ImportModule(Module module)
        {
            var type = module.GetTypes();
            foreach(var t in type)
            {
                if(!t.IsPublic)
                {
                    continue;
                }
                var ns = GetNameSpace(t.Namespace.Split('.').ToList());
                if(t.IsEnum)
                {
                    ns.Append(ImportEnum(t));
                }
                else
                {
                    ns.Append(ImportType(t));
                }
            }
            var method = module.GetMethods();
            foreach(var m in method)
            {
                if(!m.IsPublic)
                {
                    continue;
                }
                Root.Append(ImportMethod(m));
            }
            var field = module.GetFields();
            foreach(var f in field)
            {
                if(!f.IsPublic)
                {
                    continue;
                }
                Root.Append(ImportField(f));
            }
        }

        private NameSpace GetNameSpace(IList<string> fullName)
        {
            ImportNameSpace current = NameSpace;
            foreach(var v in fullName)
            {
                current = current.GetImportNameSpace(v);
            }
            return current.NameSpace;
        }

        private void AppendPeir(Scope scope, dynamic info)
        {
            Peir.Add(new InfoPeir { Scope = scope, Info = info });
        }

        private DeclateClass ImportType(Type type)
        {
            var generic = CreateGenericList(type.GetGenericList());
            var inherit = CreateInheritList(type.GetInheritList());
            var exp = new DirectiveList { IsImport = true };
            var ctor = type.GetConstructors();
            foreach(var c in ctor)
            {
                //exp.Append(ConvertConstructor(c));
            }
            var eve = type.GetEvents();
            foreach(var e in eve)
            {
                exp.Append(ConvertEvent(e));
            }
            var property = type.GetProperties();
            foreach(var p in property)
            {
                if (p.GetMethod != null)
                {
                    exp.Append(ImportMethod(p.GetMethod));
                }
                if(p.SetMethod != null)
                {
                    exp.Append(ImportMethod(p.SetMethod));
                }
            }
            var method = type.GetMethods();
            foreach(var m in method)
            {
                exp.Append(ImportMethod(m));
            }
            var field = type.GetFields();
            foreach(var f in field)
            {
                exp.Append(ImportField(f));
            }
            var nested = type.GetNestedTypes();
            foreach(var n in nested)
            {
                exp.Append(ImportType(n));
            }
            DeclateClass result = new DeclateClass { Name = type.GetPureName(), Generic = generic, Inherit = inherit, Block = exp, IsImport = true };
            AppendPeir(result, type);
            foreach (var c in ctor)
            {
                AppendPeir(result, c);
            }
            return result;
        }

        private TupleList CreateGenericList(List<Type> generic)
        {
            var tuple = new TupleList { IsImport = true };
            foreach(var v in generic)
            {
                var temp = ConvertGeneric(v);
                tuple.Append(temp);
            }
            return tuple;
        }

        private DeclateGeneric ConvertGeneric(Type generic)
        {
            var ident = new IdentifierAccess { Value = generic.GetPureName(), IsImport = true };
            return new DeclateGeneric { Ident = ident, IsImport = true };//型制約を扱えるようにする必要あり。
        }

        private TupleList CreateInheritList(List<Type> inherit)
        {
            var tuple = new TupleList { IsImport = true };
            foreach (var v in inherit)
            {
                var temp = CreateAccess(v);
                tuple.Append(temp);
            }
            return tuple;
        }

        private Element CreateAccess(Type type)
        {
            return CreateAccess(type.GetPureFullName());
        }

        private Element CreateAccess(List<string> pureFullName)
        {
            if (pureFullName.Count > 1)
            {
                var right = new IdentifierAccess { Value = pureFullName[pureFullName.Count - 1], IsImport = true };
                pureFullName.RemoveAt(pureFullName.Count - 1);
                var left = CreateAccess(pureFullName);
                return new MemberAccess { Left = left, Right = right, IsImport = true };
            }
            else if (pureFullName.Count > 0)
            {
                return new IdentifierAccess { Value = pureFullName[0], IsImport = true };
            }
            else
            {
                return null;
            }
        }

        private Element ConvertConstructor(ConstructorInfo ctor)
        {
            var argument = CreateArgumentList(ctor.GetArgumentList());
            var expl = CreateAccess(ctor.DeclaringType);
            DeclateRoutine result = new DeclateRoutine { Name = ctor.Name, Argument = argument, ExplicitType = expl, IsImport = true };
            AppendPeir(result, ctor);
            return result;
        }

        private Element ConvertEvent(EventInfo eve)
        {
            var ident = new IdentifierAccess { Value = eve.Name, IsImport = true };
            var expl = CreateAccess(eve.DeclaringType);
            DeclateVariant result = new DeclateVariant { Ident = ident, ExplicitType = expl, IsImport = true };
            AppendPeir(result, eve);
            return result;
        }

        private DeclateEnum ImportEnum(Type enumType)
        {
            var ident = new IdentifierAccess { Value = enumType.GetPureName(), IsImport = true };
            var result = new DeclateEnum { Ident = ident, IsImport = true }; //後で作る。
            AppendPeir(result, enumType);
            return result;
        }

        private DeclateRoutine ImportMethod(MethodInfo method)
        {
            var generic = CreateGenericList(method.GetGenericList());
            var argument = CreateArgumentList(method.GetArgumentList());
            var expl = CreateAccess(method.ReturnType);
            DeclateRoutine result = new DeclateRoutine { Name = method.GetPureName(), Generic = generic, Argument = argument, ExplicitType = expl, IsImport = true };
            AppendPeir(result, method);
            return result;
        }

        private TupleList CreateArgumentList(List<ParameterInfo> argument)
        {
            var tuple = new TupleList { IsImport = true };
            foreach (var v in argument)
            {
                var temp = ConvertArgument(v);
                tuple.Append(temp);
            }
            return tuple;
        }

        private DeclateArgument ConvertArgument(ParameterInfo argument)
        {
            var ident = new IdentifierAccess { Value = argument.Name, IsImport = true };
            var expl = CreateAccess(argument.ParameterType);
            DeclateArgument result = new DeclateArgument { Ident = ident, ExplicitType = expl, IsImport = true };
            AppendPeir(result, argument);
            return result;
        }

        private DeclateVariant ImportField(FieldInfo field)
        {
            var ident = new IdentifierAccess { Value = field.Name, IsImport = true };
            var expl = CreateAccess(field.FieldType);
            DeclateVariant result = new DeclateVariant { Ident = ident, ExplicitType = expl, IsImport = true };
            AppendPeir(result, field);
            return result;
        }

        private struct InfoPeir
        {
            public Scope Scope { get; set; }
            public dynamic Info { get; set; }
        }

        private class ImportNameSpace
        {
            public NameSpace NameSpace { get; set; }
            private Dictionary<string, ImportNameSpace> Child;

            public ImportNameSpace()
            {
                Child = new Dictionary<string, ImportNameSpace>();
            }

            public ImportNameSpace GetImportNameSpace(string name)
            {
                ImportNameSpace result;
                if(Child.TryGetValue(name, out result))
                {
                    return result;
                }
                NameSpace temp = new NameSpace { Name = name, IsImport = true };
                NameSpace.Append(temp);
                result = new ImportNameSpace { NameSpace = temp };
                Child.Add(name, result);
                return result;
            }
        }
    }

    internal static class CilImportExtension
    {
        public static string GetPureName(this MethodInfo method)
        {
            return method.Name.Split('`')[0];
        }

        public static string GetPureName(this Type type)
        {
            if(type.IsByRef)
            {
                return type.GetElementType().GetPureName();
            }
            if(type.IsPointer)
            {
                return type.GetElementType().GetPureName();
            }
            if(type.IsArray)
            {
                return type.GetElementType().GetPureName();
            }
            return type.Name.Split('`')[0];
        }

        public static List<string> GetPureFullName(this Type type)
        {
            if (type.IsByRef)
            {
                return type.GetElementType().GetPureFullName();
            }
            if (type.IsPointer)
            {
                return type.GetElementType().GetPureFullName();
            }
            if (type.IsArray)
            {
                return type.GetElementType().GetPureFullName();
            }
            if (type.IsGenericParameter)
            {
                var temp = new List<string>();
                temp.Add(type.GetPureName());
                return temp;
            }
            var result = new List<string>();
            result.Add("global");
            result.AddRange(type.Namespace.Split('.'));
            result.AddRange(type.GetNestedName());
            return result;
        }

        public static List<string> GetNestedName(this Type type)
        {
            var result = new List<string>();
            if(type.IsNested)
            {
                result.AddRange(type.DeclaringType.GetNestedName());
            }
            result.Add(type.GetPureName());
            return result;
        }

        public static List<Type> GetInheritList(this Type type)
        {
            var result = new List<Type>();
            var baseType = type.BaseType;
            if (baseType != null)
            {
                result.Add(baseType);
            }
            foreach(var t in type.GetInterfaces())
            {
                if(!t.IsPublic)
                {
                    continue;
                }
                result.Add(t);
            }
            return result;
        }

        public static List<Type> GetGenericList(this Type type)
        {
            return type.GetGenericArguments().ToList();
        }

        public static List<Type> GetGenericList(this MethodInfo method)
        {
            return method.GetGenericArguments().ToList();
        }

        public static List<ParameterInfo> GetArgumentList(this MethodBase method)
        {
            return method.GetParameters().ToList();
        }
    }
}
