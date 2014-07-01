using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class MemberAccess : Element
    {
        public Element Access { get; set; }
        public string Member { get; set; }
        private OverLoad _Reference;

        public override IDataType ReturnType
        {
            get { return CallScope.CallReturnType; }
        }

        public Scope CallScope
        {
            get { return Reference.CallSelect().Call; }
        }

        public override OverLoad Reference
        {
            get
            {
                if(_Reference == null)
                {
                    var scope = (Scope)Access.ReturnType;
                    _Reference = scope.NameResolution(Member);
                }
                return _Reference;
            }
        }

        public override int Count
        {
            get { return 1; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Access;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal override void CheckSyntax()
        {
            base.CheckSyntax();
            if (Reference.IsUndefined)
            {
                CompileError("undefined-identifier");
            }
            var s = Reference.CallSelect().Call;
            if (s.IsAnyAttribute(AttributeType.Private) && !HasCurrentAccess(s.CurrentScope))
            {
                CompileError("not-accessable");
            }
        }
    }
}
