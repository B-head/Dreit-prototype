using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateArgument : DeclateVariant
    {
        public DeclateArgument(TextPosition tp, TupleList attr, IdentifierAccess ident, IdentifierAccess expl)
            :base(tp, attr, ident, expl, false)
        {

        }
    }
}
