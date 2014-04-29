using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class MemberAccess : DyadicExpression, IAccess
    {
        private OverLoadScope _Reference;

        public override DataType DataType
        {
            get 
            {
                if(_Reference == null)
                {
                    GetReference(CurrentScope);
                }
                return Right.DataType; 
            }
        }

        public OverLoadScope Reference
        {
            get
            {
                if(_Reference == null)
                {
                    GetReference(CurrentScope);
                }
                return _Reference;
            }
        }

        public void GetReference(Scope scope)
        {
            var left = Left as IAccess;
            var right = Right as IAccess;
            left.GetReference(scope);
            right.GetReference(Left.DataType);
            _Reference = right.Reference;
        }

        internal override void CheckDataType()
        {
            if (_Reference == null)
            {
                GetReference(CurrentScope);
            }
            base.CheckDataType();
        }
    }
}
