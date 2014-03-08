using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class DyadicCalculate : DyadicExpression
    {
        public Scope ReferOp { get; private set; }

        internal override Scope DataType
        {
            get { return Left.DataType; }
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            string callName = string.Empty;
            switch(Operator)
            {
                case TokenType.Add: callName = "+"; break;
                case TokenType.Subtract: callName = "-"; break;
                case TokenType.Multiply: callName = "*"; break;
                case TokenType.Divide: callName = "/"; break;
                case TokenType.Modulo: callName = "%"; break;
                default: throw new Exception();
            }
            ReferOp = DataType.NameResolution(callName);
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            Scope l = Left.DataType;
            Scope r = Right.DataType;
            if (l != r)
            {
                CompileError(l + " 型と " + r + " 型を演算することは出来ません。");
            }
        }
    }
}
