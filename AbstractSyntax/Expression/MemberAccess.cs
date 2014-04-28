using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class MemberAccess : DyadicExpression, IAccess
    {
        public override DataType DataType
        {
            get { return Right.DataType; }
        }

        public OverLoadScope Reference
        {
            get
            {
                var right = Right as IAccess;
                return right.Reference; 
            }
        }

        internal override void SpreadReference(Scope scope)
        {
            Left.SpreadReference(scope);
            Right.SpreadReference(Left.DataType);
        }
    }
}
