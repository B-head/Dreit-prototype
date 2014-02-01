using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.CilTranslate;

namespace Dlight
{
    class RightAssign : DyadicExpression
    {
        public override void CheckSemantic()
        {
            if (Right != null && !Right.IsReference)
            {
                CompileError("割り当て可能な式である必要があります。");
            }
            base.CheckSemantic();
        }

        public override void CheckDataType()
        {
            if (Right != null && Left != null)
            {
                Translator temp = Left.GetDataType();
                Right.CheckDataTypeAssign(temp);
            }
            base.CheckDataType();
        }

        public override Translator GetDataType()
        {
            return Left.GetDataType();
        }

        public override void Translate()
        {
            Left.Translate();
            Right.TranslateAssign();
            Right.Translate();
        }
    }
}
