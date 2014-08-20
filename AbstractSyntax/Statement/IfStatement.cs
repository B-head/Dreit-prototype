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
        public ProgramContext Then { get; private set; }
        public ProgramContext Else { get; private set; }
        private TypeSymbol _ReturnType;

        public IfStatement(TextPosition tp, Element cond, ProgramContext then, ProgramContext els)
            :base(tp)
        {
            Condition = cond;
            Then = then;
            Else = els;
            AppendChild(Condition);
            AppendChild(Then);
            AppendChild(Else);
        }

        public bool IsDefinedElse
        {
            get { return Else != null; }
        }

        public override TypeSymbol ReturnType
        {
            get 
            {
                if(_ReturnType != null)
                {
                    return _ReturnType;
                }
                var a = BlockReturnType(Then);
                var b = BlockReturnType(Else);
                if (a == b || Else == null)
                {
                    _ReturnType = a;
                }
                else
                {
                    _ReturnType = Root.ErrorType;
                }
                return _ReturnType; 
            }
        }

        private TypeSymbol BlockReturnType(ProgramContext block)
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
