using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class Expression : Element
    {
        public override void CheckSemantic()
        {
            foreach (Element v in EnumChild())
            {
                if (v == null)
                {
                    CompileError("式が必要です。");
                    continue;
                }
            }
            base.CheckSemantic();
        }
    }

    class ExpressionSet : Element
    {
        public List<Element> Child { get; set; }
        public List<TokenType> ExpType { get; set; }

        public override int ChildCount
        {
            get { return Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return Child[index];
        }

        public override string ElementInfo()
        {
            StringBuilder result = new StringBuilder(base.ElementInfo());
            for (int i = 0; i < ExpType.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(", ");
                }
                result.Append(Enum.GetName(typeof(TokenType), ExpType[i]));
            }
            return result.ToString();
        }
    }

    class Binomial : Element
    {
        public Element Left { get; set; }
        public Element Right { get; set; }
        public TokenType Operation { get; set; }

        public override int ChildCount
        {
            get { return 2; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override string ElementInfo()
        {
            return base.ElementInfo() + Enum.GetName(typeof(TokenType), Operation);
        }

        public override void CheckDataType()
        {
            string l = Left.GetDataType();
            string r = Right.GetDataType();
            if(l != r)
            {
                CompileError(l + " 型と " + r + " 型を演算することは出来ません。");
            }
            base.CheckDataType();
        }

        public override string GetDataType()
        {
            // 式の結果の型を渡すようにしないと・・・
            return Left.GetDataType();
        }

        public override void Translate()
        {
            base.Translate();
            string type = Left.GetDataType();
            Trans.GenelateOperate(type, Operation);
        }
    }
}
