using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class MemberAccess : Element, IAccess
    {
        public Element Access { get; set; }
        public IdentifierAccess Ident { get; set; }
        private OverLoad _Reference;

        public override IDataType DataType
        {
            get 
            {
                if(_Reference == null)
                {
                    RefarenceResolution(CurrentScope);
                }
                return Ident.DataType; 
            }
        }

        public Scope CallScope
        {
            get { return Reference.TypeSelect().Call; }
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

        public override int Count
        {
            get { return 2; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Access;
                    case 1: return Ident;
                    default: throw new ArgumentOutOfRangeException();
                }
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
            var acs = Access as IAccess;
            if (acs != null)
            {
                acs.RefarenceResolution(scope);
            }
            Ident.RefarenceResolution(Access.DataType);
            _Reference = Ident.Reference;
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
