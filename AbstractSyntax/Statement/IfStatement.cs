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
        private Lazy<IDataType> LazyReturnType;

        public IfStatement(TextPosition tp, Element cond, DirectiveList than, DirectiveList els)
            :base(tp)
        {
            Condition = cond;
            Than = than;
            Else = els;
            LazyReturnType = new Lazy<IDataType>(InitReturnType);
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
            get { return LazyReturnType.Value; }
        } 

        private IDataType InitReturnType()
        {
            var a = BlockReturnType(Than);
            var b = BlockReturnType(Else);
            if(a == b)
            {
                return a;
            }
            else
            {
                return Root.Unknown;
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
