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
                case TokenType.Plus: break;
                case TokenType.Minus: cg.GenerateControl(OpCodes.Neg); break;
                case TokenType.Not: BuildNot(cg); break;
                case TokenType.Add: cg.GenerateControl(OpCodes.Add); break;
                case TokenType.Subtract: cg.GenerateControl(OpCodes.Sub); break;
                case TokenType.Multiply: cg.GenerateControl(OpCodes.Mul); break;
                case TokenType.Divide: cg.GenerateControl(OpCodes.Div); break;
                case TokenType.Modulo: cg.GenerateControl(OpCodes.Rem); break;
                case TokenType.Equal: cg.GenerateControl(OpCodes.Ceq); break;
                case TokenType.NotEqual: cg.GenerateControl(OpCodes.Ceq); BuildNot(cg); break;
                case TokenType.LessThan: cg.GenerateControl(OpCodes.Clt); break;
                case TokenType.LessThanOrEqual: cg.GenerateControl(OpCodes.Cgt); BuildNot(cg); break;
                case TokenType.GreaterThan: cg.GenerateControl(OpCodes.Cgt); break;
                case TokenType.GreaterThanOrEqual: cg.GenerateControl(OpCodes.Clt); BuildNot(cg); break;
                default: throw new ArgumentException();
            }
        }

        private void BuildNot(CodeGenerator cg)
        {
            cg.GenerateControl(OpCodes.Ldc_I4_0); cg.GenerateControl(OpCodes.Ceq);
        }
    }
}
