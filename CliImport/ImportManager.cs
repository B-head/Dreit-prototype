﻿using System;
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
                ImportModule(root, m);
            }
        }

        private static void ImportModule(Root root, Module module)
        {
            var type = module.GetTypes();
            foreach(var t in type)
            {
                if(!t.IsPublic)
                {
                    continue;
                }
                var ns = GetNameSpace(root, t.Namespace.Split('.').ToList());
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
                root.Append(ImportMethod(m));
            }
            var field = module.GetFields();
            foreach(var f in field)
            {
                if(!f.IsPublic)
                {
                    continue;
                }
                root.Append(ImportField(f));
            }
        }

        private static NameSpace GetNameSpace(NameSpace parent, IList<string> fullName)
        {
            if (fullName.Count > 0)
            {
                NameSpace current;
                List<Scope> temp;
                if(parent.ScopeChild.TryGetValue(fullName[0], out temp))
                {
                    current = (NameSpace)temp[0];
                }
                else
                {
                    current = new NameSpace { Name = fullName[0] };
                    parent.Append(current);
                    parent.AddChild(current);
                }
                fullName.RemoveAt(0);
                return GetNameSpace(current, fullName);
            }
            else
            {
                return parent;
            }
        }

        private static DeclateClass ImportType(Type type)
        {
            var ident = new Identifier { Value = type.GetPureName() };
            var generic = CreateGenericList(type.GetGenericList());
            var inherit = CreateInheritList(type.GetInheritList());
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
            return new DeclateClass { Ident = ident, GenericList = generic, InheritList = inherit, Block = exp, IsImport = true };
        }

        private static Element CreateGenericList(List<Type> generic)
        {
            if(generic.Count > 1)
            {
                var left = ConvertGeneric(generic[0]);
                generic.RemoveAt(0);
                var right = CreateGenericList(generic);
                return new TupleList { Left = left, Right = right };
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

        private static DeclareVariant ConvertGeneric(Type generic)
        {
            var ident = new Identifier { Value = generic.GetPureName() };
            return new DeclareVariant { Ident = ident, IsImport = true };//型制約を扱えるようにする必要あり。
        }

        private static Element CreateInheritList(List<Type> inherit)
        {
            if (inherit.Count > 1)
            {
                var left = CreateAccess(inherit[0]);
                inherit.RemoveAt(0);
                var right = CreateInheritList(inherit);
                return new TupleList { Left = left, Right = right };
            }
            else if (inherit.Count > 0)
            {
                return CreateAccess(inherit[0]);
            }
            else
            {
                return null;
            }
        }

        private static Element CreateAccess(Type type)
        {
            return CreateAccess(type.GetPureFullName());
        }

        private static Element CreateAccess(List<string> pureFullName)
        {
            if (pureFullName.Count > 1)
            {
                var right = new Identifier { Value = pureFullName[pureFullName.Count - 1] };
                pureFullName.RemoveAt(pureFullName.Count - 1);
                var left = CreateAccess(pureFullName);
                return new MemberAccess { Left = left, Right = right };
            }
            else if (pureFullName.Count > 0)
            {
                return new Identifier { Value = pureFullName[0] };
            }
            else
            {
                return null;
            }
        }

        private static Element ConvertConstructor(ConstructorInfo ctor)
        {
            var ident = new Identifier { Value = ctor.Name };
            var argument = CreateArgumentList(ctor.GetArgumentList());
            var expl = CreateAccess(ctor.DeclaringType);
            return new DeclateRoutine { Ident = ident, ArgumentList = argument, ExplicitResultType = expl, IsImport = true };
        }

        private static Element ConvertEvent(EventInfo eve)
        {
            var ident = new Identifier { Value = eve.Name };
            var expl = CreateAccess(eve.DeclaringType);
            return new DeclareVariant { Ident = ident, ExplicitVariantType = expl, IsImport = true };
        }

        private static Element ImportEnum(Type enumType)
        {
            var ident = new Identifier { Value = enumType.GetPureName() };
            return new DeclareVariant { Ident = ident, IsImport = true };
        }

        private static DeclateRoutine ImportMethod(MethodInfo method)
        {
            var ident = new Identifier { Value = method.GetPureName() };
            var generic = CreateGenericList(method.GetGenericList());
            var argument = CreateArgumentList(method.GetArgumentList());
            var expl = CreateAccess(method.ReturnType);
            return new DeclateRoutine { Ident = ident, GenericList = generic, ArgumentList = argument, ExplicitResultType = expl, IsImport = true };
        }

        private static Element CreateArgumentList(List<ParameterInfo> argument)
        {
            if (argument.Count > 1)
            {
                var left = ConvertArgument(argument[0]);
                argument.RemoveAt(0);
                var right = CreateArgumentList(argument);
                return new TupleList { Left = left, Right = right };
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

        private static DeclareVariant ConvertArgument(ParameterInfo argument)
        {
            var ident = new Identifier { Value = argument.Name };
            var expl = CreateAccess(argument.ParameterType);
            return new DeclareVariant { Ident = ident, ExplicitVariantType = expl, IsImport = true };
        }

        private static DeclareVariant ImportField(FieldInfo field)
        {
            var ident = new Identifier { Value = field.Name };
            var expl = CreateAccess(field.FieldType);
            return new DeclareVariant { Ident = ident, ExplicitVariantType = expl, IsImport = true };
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
            var result = type.Namespace.Split('.').ToList();
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
