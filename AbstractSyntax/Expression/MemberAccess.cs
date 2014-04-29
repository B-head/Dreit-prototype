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
            get { return GetReference(CurrentScope).GetDataType(); }
        }

        public OverLoadScope Reference
        {
            get { return GetReference(CurrentScope); }
        }

        public OverLoadScope GetReference(Scope scope)
        {
            var left = Left as IAccess;
            var right = Right as IAccess;
            var refer = left.GetReference(scope);
            return right.GetReference(refer.GetDataType());
        }
    }
}
