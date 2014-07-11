using AbstractSyntax;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    switch (a.Attr)
                    {
                        case AttributeType.Public: ret |= TypeAttributes.Public | TypeAttributes.NestedAssembly; break;
                        case AttributeType.Protected: ret |= TypeAttributes.NotPublic | TypeAttributes.NestedFamily; break;
                        case AttributeType.Private: ret |= TypeAttributes.NotPublic | TypeAttributes.NestedPrivate; break;
                    }
                }
                else
                {
                    switch (a.Attr)
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
                switch (a.Attr)
                {
                    case AttributeType.Static: ret |= MethodAttributes.Static; break;
                    case AttributeType.Public: ret |= MethodAttributes.Assembly; break;
                    case AttributeType.Protected: ret |= MethodAttributes.Family; break;
                    case AttributeType.Private: ret |= MethodAttributes.Private; break;
                }
            }
            return ret;
        }

        public static FieldAttributes MakeFieldAttributes(this IReadOnlyList<Scope> attr)
        {
            FieldAttributes ret = 0;
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                switch (a.Attr)
                {
                    case AttributeType.Static: ret |= FieldAttributes.Static; break;
                    case AttributeType.Public: ret |= FieldAttributes.Assembly; break;
                    case AttributeType.Protected: ret |= FieldAttributes.Family; break;
                    case AttributeType.Private: ret |= FieldAttributes.Private; break;
                }
            }
            return ret;
        }
    }
}
