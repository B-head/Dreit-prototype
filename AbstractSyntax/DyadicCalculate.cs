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
        internal override void CheckDataType()
        {
            Scope l = Left.GetDataType();
            Scope r = Right.GetDataType();
            if (l != r)
            {
                CompileError(l + " 型と " + r + " 型を演算することは出来ません。");
            }
            base.CheckDataType();
        }

        internal override Scope GetDataType()
        {
            // 式の結果の型を渡すようにしないと・・・
            return Left.GetDataType();
        }

        internal override void Translate()
        {
            base.Translate();
            Scope type = Left.GetDataType();
            string callName = string.Empty;
            switch(Operation)
            {
                case TokenType.Add: callName = "opAdd"; break;
                case TokenType.Multiply: callName = "opMultiply"; break;
                default: throw new Exception();
            }
            Trans.GenelateCall(type.NameResolution(callName).FullPath);
        }
    }
}
