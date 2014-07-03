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
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
            if (Reference.IsUndefined)
            {
                CompileError("undefined-identifier");
            }
            if (CallScope.IsAnyAttribute(AttributeType.Private) && !HasCurrentAccess(CallScope.CurrentScope))
            {
                CompileError("not-accessable");
            }
            if (CallScope.IsAnyAttribute(AttributeType.Protected) && !HasCurrentAccess(CallScope.CurrentScope))
            {
                CompileError("not-accessable");
            }
            if (CallScope.IsInstanceMember && Access.ReturnType is TypeofClassSymbol)
            {
                CompileError("not-accessable");
            }
            if (CallScope.IsStaticMember && !(Access.ReturnType is TypeofClassSymbol))
            {
                CompileError("not-accessable");
            }
            if (CallScope.IsStaticMember && !ContainClass(CallScope.GetParent<ClassSymbol>(), Access.ReturnType))
            {
                CompileError("undefined-identifier");
            }
        }

        private static bool ContainClass(ClassSymbol cls, IDataType type)
        {
            if(type is ClassSymbol)
            {
                return cls == type;
            }
            else if(type is TypeofClassSymbol)
            {
                return cls.TypeofSymbol == type;
            }
            else
            {
                return false;
            }
        }
    }
}
