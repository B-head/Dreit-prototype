using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class CastStructure : BuilderStructure
    {
        public PrimitiveType PrimitiveType { get; private set; }

        public CastStructure(PrimitiveType type)
        {
            PrimitiveType = type;
        }

        internal override void BuildCall(CodeGenerator cg)
        {
            switch (PrimitiveType)
            {
                case PrimitiveType.Integer8: cg.GenerateCode(OpCodes.Conv_I1); break;
                case PrimitiveType.Integer16: cg.GenerateCode(OpCodes.Conv_I2); break;
                case PrimitiveType.Integer32: cg.GenerateCode(OpCodes.Conv_I4); break;
                case PrimitiveType.Integer64: cg.GenerateCode(OpCodes.Conv_I8); break;
                case PrimitiveType.Natural8: cg.GenerateCode(OpCodes.Conv_U1); break;
                case PrimitiveType.Natural16: cg.GenerateCode(OpCodes.Conv_U2); break;
                case PrimitiveType.Natural32: cg.GenerateCode(OpCodes.Conv_U4); break;
                case PrimitiveType.Natural64: cg.GenerateCode(OpCodes.Conv_U8); break;
                case PrimitiveType.Binary32: cg.GenerateCode(OpCodes.Conv_R4); break;
                case PrimitiveType.Binary64: cg.GenerateCode(OpCodes.Conv_R8); break;
                default: throw new ArgumentException();
            }
        }
    }
}
