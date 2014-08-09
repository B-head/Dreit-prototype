using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class LoopStatement : Scope
    {
        public Element Condition { get; set; }
        public Element On { get; set; }
        public Element By { get; set; }
        public ExpressionList Block { get; set; }

        public LoopStatement(TextPosition tp, Element cond, Element on, Element by, ExpressionList block)
            :base(tp)
        {
            Condition = cond;
            On = on;
            By = by;
            Block = block;
            AppendChild(Condition);
            AppendChild(On);
            AppendChild(By);
            AppendChild(Block);
        }

        public bool IsDefinedCondition
        {
            get { return Condition != null; }
        }

        public bool IsDefinedOn
        {
            get { return On != null; }
        }

        public bool IsDefinedBy
        {
            get { return By != null; }
        }

    }
}
