using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax
{
    public class CalculatePragma : DeclateRoutine
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
    }

    public enum CalculatePragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Cast,
    }
}
