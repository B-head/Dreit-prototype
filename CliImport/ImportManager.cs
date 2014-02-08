using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AbstractSyntax;

namespace CliImport
{
    public static class ImportManager
    {
        public static void ImportAssembly(Root root, Assembly assembly)
        {
            var module = assembly.GetModules();
            foreach(var m in module)
            {
                root.Append(ImportModule(m)); 
            }
        }

        public static NameSpace ImportModule(Module module)
        {
            var exp = new ExpressionList();
            var type = module.GetTypes();
            foreach(var t in type)
            {
                if(!t.IsPublic)
                {
                    continue;
                }
                if(t.IsEnum)
                {
                    exp.Append(ImportEnum(t));
                }
                else
                {
                    exp.Append(ImportType(t));
                }
            }
            var method = module.GetMethods();
            foreach(var m in method)
            {
                if(!m.IsPublic)
                {
                    continue;
                }
                exp.Append(ImportMethod(m));
            }
            var field = module.GetFields();
            foreach(var f in field)
            {
                if(!f.IsPublic)
                {
                    continue;
                }
                exp.Append(ImportField(f));
            }
            return new NameSpace { ExpList = exp };
        }

        public static DeclateClass ImportType(Type type)
        {
            var ident = new Identifier { Value = type.Name };
            var generic = CreateGenericList(type.GetGenericArguments().ToList());
            var inherit = CreateInheritList(type.BaseType, type.GetInterfaces().ToList());
            var exp = new ExpressionList();
            var ctor = type.GetConstructors();
            foreach(var c in ctor)
            {
                exp.Append(ConvertConstructor(c));
            }
            var eve = type.GetEvents();
            foreach(var e in eve)
            {
                exp.Append(ConvertEvent(e));
            }
            var property = type.GetProperties();
            foreach(var p in property)
            {
                exp.Append(ConvertProperty(p));
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
            return new DeclateClass { Ident = ident, GenericList = generic, InheritList = inherit, Block = exp };
        }

        public static Element CreateGenericList(List<Type> generic)
        {
            if(generic.Count > 1)
            {
                var left = ConvertGeneric(generic[0]);
                generic.RemoveAt(0);
                var right = CreateGenericList(generic);
                return new TupleExpression { Left = left, Right = right };
            }
            else if (generic.Count > 0)
            {
                return ConvertGeneric(generic[0]);
            }
            else
            {
                return null;
            }
        }

        public static DeclareVariant ConvertGeneric(Type generic)
        {
            var ident = new Identifier { Value = generic.Name };
            return new DeclareVariant { Ident = ident };//型制約を扱えるようにする必要あり。
        }

        public static Element CreateInheritList(Type baseType, List<Type> interfaceType)
        {
            if (interfaceType.Count > 0)
            {
                var left = new Identifier { Value = baseType.Name };
                var right = CreateInheritList(interfaceType);
                return new TupleExpression { Left = left, Right = right };
            }
            else if (baseType != null)
            {
                return new Identifier { Value = baseType.Name };
            }
            else
            {
                return null;
            }
        }

        public static Element CreateInheritList(List<Type> interfaceType)
        {
            if (interfaceType.Count > 1)
            {
                var left = new Identifier { Value = interfaceType[0].Name };
                interfaceType.RemoveAt(0);
                var right = CreateInheritList(interfaceType);
                return new TupleExpression { Left = left, Right = right };
            }
            else if (interfaceType.Count > 0)
            {
                return new Identifier { Value = interfaceType[0].Name };
            }
            else
            {
                return null;
            }
        }

        public static Element ConvertConstructor(ConstructorInfo ctor)
        {
            return null;
        }

        public static Element ConvertEvent(EventInfo eve)
        {
            return null;
        }

        public static Element ConvertProperty(PropertyInfo property)
        {
            return null;
        }

        public static Element ImportEnum(Type enumType)
        {
            return null;
        }

        public static DeclateRoutine ImportMethod(MethodInfo method)
        {
            var ident = new Identifier { Value = method.Name };
            var generic = CreateGenericList(method.GetGenericArguments().ToList());
            var argument = CreateArgumentList(method.GetParameters().ToList());
            var expl = new Identifier { Value = method.ReturnType.Name };
            return new DeclateRoutine { Ident = ident, GenericList = generic, ArgumentList = argument, ExplicitResultType = expl };
        }

        public static Element CreateArgumentList(List<ParameterInfo> argument)
        {
            if (argument.Count > 1)
            {
                var left = ConvertArgument(argument[0]);
                argument.RemoveAt(0);
                var right = CreateArgumentList(argument);
                return new TupleExpression { Left = left, Right = right };
            }
            else if (argument.Count > 0)
            {
                return ConvertArgument(argument[0]);
            }
            else
            {
                return null;
            }
        }

        public static DeclareVariant ConvertArgument(ParameterInfo argument)
        {
            var ident = new Identifier { Value = argument.Name };
            var expl = new Identifier { Value = argument.ParameterType.Name };
            return new DeclareVariant { Ident = ident, ExplicitDataType = expl };
        }

        public static DeclareVariant ImportField(FieldInfo field)
        {
            var ident = new Identifier { Value = field.Name };
            var expl = new Identifier { Value = field.FieldType.Name };
            return new DeclareVariant { Ident = ident, ExplicitDataType = expl };
        }
    }
}
