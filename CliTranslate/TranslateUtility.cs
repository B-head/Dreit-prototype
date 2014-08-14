using AbstractSyntax;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    static class TranslateUtility
    {
        public static TypeAttributes MakeTypeAttributes(this IReadOnlyList<Scope> attr, bool isTrait = false, bool isNested = false)
        {
            TypeAttributes ret = isTrait ? TypeAttributes.Interface | TypeAttributes.Abstract : TypeAttributes.Class;
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                if (isNested)
                {
                    switch (a.AttributeType)
                    {
                        case AttributeType.Public: ret |= TypeAttributes.Public | TypeAttributes.NestedAssembly; break;
                        case AttributeType.Protected: ret |= TypeAttributes.NotPublic | TypeAttributes.NestedFamily; break;
                        case AttributeType.Private: ret |= TypeAttributes.NotPublic | TypeAttributes.NestedPrivate; break;
                    }
                }
                else
                {
                    switch (a.AttributeType)
                    {
                        case AttributeType.Public: ret |= TypeAttributes.Public; break;
                        case AttributeType.Protected: ret |= TypeAttributes.NotPublic; break;
                        case AttributeType.Private: ret |= TypeAttributes.NotPublic; break;
                    }
                }
            }
            return ret;
        }

        public static MethodAttributes MakeMethodAttributes(this IReadOnlyList<Scope> attr, bool isVirtual = false, bool isAbstract = false)
        {
            MethodAttributes ret = MethodAttributes.ReuseSlot;
            if(isVirtual)
            {
                ret |= MethodAttributes.Virtual;
            }
            if(isAbstract)
            {
                ret |= MethodAttributes.Abstract;
            }
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                switch (a.AttributeType)
                {
                    case AttributeType.Static: ret |= MethodAttributes.Static; break;
                    case AttributeType.Public: ret |= MethodAttributes.Assembly; break;
                    case AttributeType.Protected: ret |= MethodAttributes.Family; break;
                    case AttributeType.Private: ret |= MethodAttributes.Private; break;
                }
            }
            return ret;
        }

        public static FieldAttributes MakeFieldAttributes(this IReadOnlyList<Scope> attr, bool isDcv)
        {
            FieldAttributes ret = 0;
            if(isDcv)
            {
                ret |= FieldAttributes.HasDefault;
            }
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                switch (a.AttributeType)
                {
                    case AttributeType.Static: ret |= FieldAttributes.Static; break;
                    case AttributeType.Public: ret |= FieldAttributes.Assembly; break;
                    case AttributeType.Protected: ret |= FieldAttributes.Family; break;
                    case AttributeType.Private: ret |= FieldAttributes.Private; break;
                }
            }
            return ret;
        }

        public static void AddImplements(this TypeBuilder builder, IReadOnlyList<TypeStructure> imp)
        {
            foreach (var v in imp)
            {
                builder.AddInterfaceImplementation(v.GainType());
            }
        }

        public static string[] ToNames(this IReadOnlyList<GenericParameterStructure> gnr)
        {
            var ret = new string[gnr.Count];
            for (var i = 0; i < gnr.Count; ++i)
            {
                ret[i] = gnr[i].Name;
            }
            return ret;
        }

        public static void RegisterBuilders(this IReadOnlyList<GenericParameterStructure> gnr, GenericTypeParameterBuilder[] builders)
        {
            for (var i = 0; i < gnr.Count; ++i)
            {
                gnr[i].RegisterBuilder(builders[i]);
            }
        }

        public static Type[] ToTypes(this IReadOnlyList<ParameterStructure> prm)
        {
            var ret = new Type[prm.Count];
            for (var i = 0; i < prm.Count; ++i)
            {
                ret[i] = prm[i].ParamType.GainType();
            }
            return ret;
        }

        public static Type[] GainTypes(this IReadOnlyList<TypeStructure> types)
        {
            var ret = new Type[types.Count];
            for (var i = 0; i < types.Count; ++i)
            {
                ret[i] = types[i].GainType();
            }
            return ret;
        }

        public static void RegisterBuilders(this IReadOnlyList<ParameterStructure> prm, MethodBuilder builder, bool isInstance)
        {
            for (var i = 0; i < prm.Count; ++i)
            {
                var p = prm[i];
                var pb = builder.DefineParameter(i + 1, p.Attributes, p.Name);
                p.RegisterBuilder(pb, isInstance);
            }
        }

        public static void RegisterBuilders(this IReadOnlyList<ParameterStructure> prm, ConstructorBuilder builder, bool isInstance)
        {
            for (var i = 0; i < prm.Count; ++i)
            {
                var p = prm[i];
                var pb = builder.DefineParameter(i + 1, p.Attributes, p.Name);
                p.RegisterBuilder(pb, isInstance);
            }
        }

        public static TypeStructure GetBaseType(this TypeStructure type)
        {
            var tit = type as GenericTypeStructure;
            if(tit == null)
            {
                return type;
            }
            if (tit.BaseType is ModifyTypeStructure)
            {
                return tit.GenericParameter[0];
            }
            else
            {
                return tit.BaseType;
            }
        }
    }
}
