using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class CalculatePragma : DeclateOperator
    {
        public CalculatePragmaType Type { get; private set; }

        public CalculatePragma(CalculatePragmaType type)
        {
            Type = type;
            Argument = new TupleList();
            Block = new DirectiveList();
        }

        public override bool IsPragma
        {
            get { return true; }
        }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
        }
    }

    public enum CalculatePragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
    }
}
