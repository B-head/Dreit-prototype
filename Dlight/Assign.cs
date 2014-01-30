using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class Assign : ExpressionSet
    {
        public bool Direction { get; set; }

        public override void CheckSemantic()
        {
            for (int i = 0; i < Child.Count; i++)
            {
                if (i < ExpType.Count)
                {
                    bool temp = GetDirection(ExpType[i]);
                    if (i == 0)
                    {
                        Direction = temp;
                    }
                    else if (Direction != temp)
                    {
                        CompileError("式中では割り当て演算子の向きが揃っている必要があります。");
                    }
                }
                if (Child[i] == null)
                {
                    continue;
                }
                if ((!Direction && i < Child.Count - 1) || (Direction && i > 0))
                {
                    if (!Child[i].IsReference)
                    {
                        CompileError("割り当て可能な式である必要があります。");
                    }
                }
            }
            base.CheckSemantic();
        }

        public override void CheckDataType()
        {
            if (Direction)
            {
                for (int i = 0; i < Child.Count - 1; i++)
                {
                    Child[i].CheckDataType();
                    FullName temp = Child[i].GetDataType();
                    Child[i + 1].CheckDataTypeAssign(temp);
                }
            }
            else
            {
                for (int i = Child.Count - 1; i >= 1; i--)
                {
                    Child[i].CheckDataType();
                    FullName temp = Child[i].GetDataType();
                    Child[i - 1].CheckDataTypeAssign(temp);
                }
            }
        }

        public override void Translate()
        {
            if (Direction)
            {
                for (int i = 0; i < Child.Count - 1; i++)
                {
                    Child[i].Translate();
                    Child[i + 1].TranslateAssign();
                }
            }
            else
            {
                for (int i = Child.Count - 1; i >= 1; i--)
                {
                    Child[i].Translate();
                    Child[i - 1].TranslateAssign();
                }
            }
        }

        private bool GetDirection(TokenType type)
        {
            switch (type)
            {
                case TokenType.LeftAssign:
                    return false;
                case TokenType.RightAssign:
                    return true;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
