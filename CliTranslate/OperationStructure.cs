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
                case TokenType.Minus: cg.GenerateCode(OpCodes.Neg); break;
                case TokenType.Not: BuildNot(cg); break;
                case TokenType.Add: cg.GenerateCode(OpCodes.Add); break;
                case TokenType.Subtract: cg.GenerateCode(OpCodes.Sub); break;
                case TokenType.Multiply: cg.GenerateCode(OpCodes.Mul); break;
                case TokenType.Divide: cg.GenerateCode(OpCodes.Div); break;
                case TokenType.Modulo: cg.GenerateCode(OpCodes.Rem); break;
                case TokenType.Equal: cg.GenerateCode(OpCodes.Ceq); break;
                case TokenType.NotEqual: cg.GenerateCode(OpCodes.Ceq); BuildNot(cg); break;
                case TokenType.LessThan: cg.GenerateCode(OpCodes.Clt); break;
                case TokenType.LessThanOrEqual: cg.GenerateCode(OpCodes.Cgt); BuildNot(cg); break;
                case TokenType.GreaterThan: cg.GenerateCode(OpCodes.Cgt); break;
                case TokenType.GreaterThanOrEqual: cg.GenerateCode(OpCodes.Clt); BuildNot(cg); break;
                default: throw new ArgumentException();
            }
        }

        private void BuildNot(CodeGenerator cg)
        {
            cg.GenerateCode(OpCodes.Ldc_I4_0); cg.GenerateCode(OpCodes.Ceq);
        }
    }
}
