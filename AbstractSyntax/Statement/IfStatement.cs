using AbstractSyntax.Expression;
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
        public ExpressionList Then { get; private set; }
        public ExpressionList Else { get; private set; }
        private Scope _ReturnType;

        public IfStatement(TextPosition tp, Element cond, ExpressionList than, ExpressionList els)
            :base(tp)
        {
            Condition = cond;
            Then = than;
            Else = els;
            AppendChild(Condition);
            AppendChild(Then);
            AppendChild(Else);
        }

        public bool IsDefinedElse
        {
            get { return Else != null; }
        }

        public override Scope ReturnType
        {
            get 
            {
                if(_ReturnType != null)
                {
                    return _ReturnType;
                }
                var a = BlockReturnType(Then);
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

        private Scope BlockReturnType(ExpressionList block)
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
