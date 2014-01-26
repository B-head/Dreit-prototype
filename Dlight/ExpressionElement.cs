using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight
{
    class ExpressionSet : Syntax
    {
        public List<Syntax> Child { get; set; }
        public List<TokenType> ExpType { get; set; }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element this[int index]
        {
            get { return Child[index]; }
        }

        public override string ElementInfo()
        {
            StringBuilder result = new StringBuilder(base.ElementInfo());
            for (int i = 0; i < ExpType.Count; i++)
            {
                if(i > 0)
                {
                    result.Append(", ");
                }
                result.Append(Enum.GetName(typeof(TokenType), ExpType[i]));
            }
            return result.ToString();
        }

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    manager.Error(ErrorInfo() + "式が必要です。");
                    continue;
                }
            }
            base.CheckSemantic(manager, scope);
        }
    }

    class Assign : ExpressionSet
    {
        public bool Direction { get; set; }

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            for (int i = 0; i < Child.Count; i++)
            {
                if (i < ExpType.Count)
                {
                    bool temp = GetDirection(ExpType[i]);
                    if(i == 0)
                    {
                        Direction = temp;
                    }
                    else if(Direction != temp)
                    {
                        manager.Error(ErrorInfo() + "式中では割り当て演算子の向きが揃っている必要があります。");
                    }
                }
                if(Child[i] == null)
                {
                    manager.Error(ErrorInfo() + "式が必要です。");
                    continue;
                }
                if ((!Direction && i < Child.Count - 1) || (Direction && i > 0))
                {
                    if (!(Child[i] is Reference))
                    {
                        manager.Error(ErrorInfo() + "割り当て可能な式である必要があります。");
                    }
                }
                Child[i].CheckSemantic(manager, scope);
            }
        }

        public override void Translate(Translator trans, Scope<Element> scope)
        {
            Reference refer = null;
            if(Direction)
            {
                for (int i = 0; i < Child.Count - 1; i++)
                {
                    Child[i].Translate(trans, scope);
                    refer = Child[i + 1] as Reference;
                    refer.Translate(trans, scope, true);
                }
            }
            else
            {
                for(int i = Child.Count - 1; i >= 1; i--)
                {
                    Child[i].Translate(trans, scope);
                    refer = Child[i - 1] as Reference;
                    refer.Translate(trans, scope, true);
                }
            }
        }

        private bool GetDirection(TokenType type)
        {
            switch(type)
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

    class Binomial : Syntax
    {
        public Syntax Left { get; set; }
        public Syntax Right { get; set; }
        public TokenType Operation { get; set; }

        public override int Count
        {
            get { return 2; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Left;
                    case 1: return Right;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string ElementInfo()
        {
            return base.ElementInfo() + Enum.GetName(typeof(TokenType), Operation);
        }

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            if (Left == null)
            {
                manager.Error(ErrorInfo() + "左辺式が必要です。");
            }
            if (Right == null)
            {
                manager.Error(ErrorInfo() + "右辺式が必要です。");
            }
            base.CheckSemantic(manager, scope);
        }

        public override void Translate(Translator trans, Scope<Element> scope)
        {
            base.Translate(trans, scope);
            trans.GenelateBinomial("Integer32", Operation);
        }
    }
}
