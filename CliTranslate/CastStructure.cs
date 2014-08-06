using AbstractSyntax.Pragma;
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
        public CastPragmaType PrimitiveType { get; private set; }

        public CastStructure(CastPragmaType type)
        {
            PrimitiveType = type;
        }

        internal override void BuildCall(CodeGenerator cg)
        {
            switch (PrimitiveType)
            {
                case CastPragmaType.Integer8: cg.GenerateControl(OpCodes.Conv_I1); break;
                case CastPragmaType.Integer16: cg.GenerateControl(OpCodes.Conv_I2); break;
                case CastPragmaType.Integer32: cg.GenerateControl(OpCodes.Conv_I4); break;
                case CastPragmaType.Integer64: cg.GenerateControl(OpCodes.Conv_I8); break;
                case CastPragmaType.Natural8: cg.GenerateControl(OpCodes.Conv_U1); break;
                case CastPragmaType.Natural16: cg.GenerateControl(OpCodes.Conv_U2); break;
                case CastPragmaType.Natural32: cg.GenerateControl(OpCodes.Conv_U4); break;
                case CastPragmaType.Natural64: cg.GenerateControl(OpCodes.Conv_U8); break;
                case CastPragmaType.Binary32: cg.GenerateControl(OpCodes.Conv_R4); break;
                case CastPragmaType.Binary64: cg.GenerateControl(OpCodes.Conv_R8); break;
                default: throw new ArgumentException();
            }
        }
    }
}
