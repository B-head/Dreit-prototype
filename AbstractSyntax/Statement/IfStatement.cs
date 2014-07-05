using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class IfStatement : Scope
    {
        public Element Condition { get; private set; }
        public DirectiveList Than { get; private set; }
        public DirectiveList Else { get; private set; }
        private IDataType _ReturnType;

        public IfStatement(TextPosition tp, Element cond, DirectiveList than, DirectiveList els)
            :base(tp)
        {
            Condition = cond;
            Than = than;
            Else = els;
        }

        public override int Count
        {
            get { return 3; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Condition;
                    case 1: return Than;
                    case 2: return Else;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsDefinedElse
        {
            get { return Else != null; }
        }

        public override IDataType ReturnType
        {
            get 
            {
                if(_ReturnType != null)
                {
                    return _ReturnType;
                }
                var a = BlockReturnType(Than);
                var b = BlockReturnType(Else);
                if (a == b)
                {
                    _ReturnType = a;
                }
                else
                {
                    _ReturnType = Root.Unknown;
                }
                return _ReturnType; 
            }
        } 

        private IDataType BlockReturnType(DirectiveList block)
        {
            if(block == null)
            {
                return Root.Void;
            }
            if(block.IsInline)
            {
                return block[0].ReturnType;
            }
            else
            {
                return Root.Void; //todo ifブロックで値を返すための構文が必要。
            }
        }
    }
}
