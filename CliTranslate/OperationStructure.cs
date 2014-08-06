using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class OperationStructure : BuilderStructure
    {
        public TokenType CalculateType { get; private set; }

        public OperationStructure(TokenType type)
        {
            CalculateType = type;
        }

        internal override void BuildCall(CodeGenerator cg)
        {
            switch (CalculateType)
            {
                case TokenType.Add: cg.GenerateControl(OpCodes.Add); break;
                case TokenType.Subtract: cg.GenerateControl(OpCodes.Sub); break;
                case TokenType.Multiply: cg.GenerateControl(OpCodes.Mul); break;
                case TokenType.Divide: cg.GenerateControl(OpCodes.Div); break;
                case TokenType.Modulo: cg.GenerateControl(OpCodes.Rem); break;
                case TokenType.Equal: cg.GenerateControl(OpCodes.Ceq); break;
                case TokenType.NotEqual: cg.GenerateControl(OpCodes.Ceq); cg.GenerateControl(OpCodes.Ldc_I4_1); cg.GenerateControl(OpCodes.Xor); break;
                case TokenType.LessThan: cg.GenerateControl(OpCodes.Clt); break;
                case TokenType.LessThanOrEqual: cg.GenerateControl(OpCodes.Cgt); cg.GenerateControl(OpCodes.Ldc_I4_1); cg.GenerateControl(OpCodes.Xor); break;
                case TokenType.GreaterThan: cg.GenerateControl(OpCodes.Cgt); break;
                case TokenType.GreaterThanOrEqual: cg.GenerateControl(OpCodes.Clt); cg.GenerateControl(OpCodes.Ldc_I4_1); cg.GenerateControl(OpCodes.Xor); break;
                default: throw new ArgumentException();
            }
        }
    }
}
