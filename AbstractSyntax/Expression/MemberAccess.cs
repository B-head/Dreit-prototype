using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class MemberAccess : DyadicExpression
    {
        public MemberAccess()
        {
            Operator = TokenType.Access;
        }

        public override DataType DataType
        {
            get { return Right.DataType; }
        }

        public override OverLoadScope Reference
        {
            get { return Right.Reference; }
        }

        internal override void SpreadReference(Scope scope)
        {
            Left.SpreadReference(scope);
            Right.SpreadReference(Left.DataType);
        }
    }
}
