using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class MemberAccess : DyadicExpression, IAccess
    {
        private OverLoad _Reference;

        public override IDataType DataType
        {
            get 
            {
                if(_Reference == null)
                {
                    RefarenceResolution(CurrentScope);
                }
                return Right.DataType; 
            }
        }

        public OverLoad Reference
        {
            get
            {
                if(_Reference == null)
                {
                    RefarenceResolution();
                }
                return _Reference;
            }
        }

        public void RefarenceResolution()
        {
            var p = Parent as IAccess;
            if (p == null)
            {
                RefarenceResolution(CurrentIScope);
            }
            else
            {
                p.RefarenceResolution();
            }
        }

        public void RefarenceResolution(IScope scope)
        {
            var left = Left as IAccess;
            var right = Right as IAccess;
            left.RefarenceResolution(scope);
            right.RefarenceResolution(Left.DataType);
            _Reference = right.Reference;
        }

        internal override void CheckDataType()
        {
            if (_Reference == null)
            {
                RefarenceResolution(CurrentScope);
            }
            base.CheckDataType();
        }
    }
}
