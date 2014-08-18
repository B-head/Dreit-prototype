using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class GenericTypeStructure : TypeStructure
    {
        public TypeStructure BaseType { get; private set; }
        public IReadOnlyList<TypeStructure> GenericParameter { get; private set; }

        public void  Initialize(TypeStructure baseType, IReadOnlyList<TypeStructure> prms, Type info = null)
        {
            BaseType = baseType;
            GenericParameter = prms;
            Info = info;
        }

        internal override Type GainType()
        {
            if(Info != null)
            {
                return Info;
            }
            var m = BaseType as ModifyTypeStructure;
            if(m == null)
            {
                Info = BaseType.GainType().MakeGenericType(GenericParameter.GainTypes());
            }
            else
            {
                var ft = GenericParameter[0].GainType();
                switch(m.ModifyType)
                {
                    case ModifyType.Refer: Info = ft.MakeByRefType(); break;
                    case ModifyType.Typeof: break;
                    case ModifyType.Nullable: break;
                    case ModifyType.Pointer: Info = ft.MakePointerType(); break;
                    case ModifyType.EmbedArray: Info = ft.MakeArrayType(); break;
                    default: throw new InvalidOperationException();
                }
            }
            return Info;
        }
    }
}
