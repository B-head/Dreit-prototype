using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.CilTranslate;

namespace Dlight
{
    class DyadicCalculate : DyadicExpression
    {
        public override void CheckDataType()
        {
            Translator l = Left.GetDataType();
            Translator r = Right.GetDataType();
            if (l != r)
            {
                CompileError(l + " 型と " + r + " 型を演算することは出来ません。");
            }
            base.CheckDataType();
        }

        public override Translator GetDataType()
        {
            // 式の結果の型を渡すようにしないと・・・
            return Left.GetDataType();
        }

        public override void Translate()
        {
            base.Translate();
            Translator type = Left.GetDataType();
            //Trans.GenelateOperate(type, Operation);
        }
    }
}
