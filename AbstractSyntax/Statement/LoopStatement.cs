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
        public bool IsLater { get; private set; }
        public Element Condition { get; private set; }
        public Element Use { get; private set; }
        public Element By { get; private set; }
        public ProgramContext Block { get; private set; }

        public LoopStatement(TextPosition tp, bool isLater, Element cond, Element use, Element by, ProgramContext block)
            :base(tp)
        {
            IsLater = isLater;
            Condition = cond;
            Use = use;
            By = by;
            Block = block;
            AppendChild(Condition);
            AppendChild(Use);
            AppendChild(By);
            AppendChild(Block);
        }

        public bool IsDefinedCondition
        {
            get { return Condition != null; }
        }

        public bool IsDefinedOn
        {
            get { return Use != null; }
        }

        public bool IsDefinedBy
        {
            get { return By != null; }
        }

    }
}
