using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DyadicCalculate : DyadicExpression
    {
        internal override Scope DataType
        {
            get { return Left.DataType; }
        }

        internal override void CheckDataType(Scope scope)
        {
            base.CheckDataType(scope);
            Scope l = Left.DataType;
            Scope r = Right.DataType;
            if (l != r)
            {
                CompileError(l + " 型と " + r + " 型を演算することは出来ません。");
            }
        }

        internal override void Translate(Translator trans)
        {
            base.Translate(trans);
            Scope type = DataType;
            string callName = string.Empty;
            switch(Operation)
            {
                case TokenType.Add: callName = "opAdd"; break;
                case TokenType.Subtract: callName = "opSubtract"; break;
                case TokenType.Multiply: callName = "opMultiply"; break;
                case TokenType.Divide: callName = "opDivide"; break;
                case TokenType.Modulo: callName = "opModulo"; break;
                default: throw new Exception();
            }
            trans.GenerateCall(type.NameResolution(callName).FullPath);
        }
    }
}
