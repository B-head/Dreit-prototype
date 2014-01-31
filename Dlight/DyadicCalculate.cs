using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class DyadicCalculate : DyadicExpression
    {
        public override void CheckDataType()
        {
            FullName l = Left.GetDataType();
            FullName r = Right.GetDataType();
            if (l != r)
            {
                CompileError(l + " 型と " + r + " 型を演算することは出来ません。");
            }
            base.CheckDataType();
        }

        public override FullName GetDataType()
        {
            // 式の結果の型を渡すようにしないと・・・
            return Left.GetDataType();
        }

        public override void Translate()
        {
            base.Translate();
            FullName type = Left.GetDataType();
            Trans.GenelateOperate(type, Operation);
        }
    }
}
