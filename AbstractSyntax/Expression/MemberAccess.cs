using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class MemberAccess : Element
    {
        public Element Access { get; private set; }
        public string Member { get; private set; }
        private OverLoad _Reference;

        public MemberAccess(TextPosition tp, Element acs, string member)
            :base(tp)
        {
            Access = acs;
            Member = member;
            AppendChild(Access);
        }

        public string Value
        {
            get { return Member; }
        }

        public override Scope ReturnType
        {
            get { return CallScope.CallReturnType; }
        }

        public Scope CallScope
        {
            get { return OverLoad.CallSelect().Call; }
        }

        public override OverLoad OverLoad
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

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
            if (OverLoad.IsUndefined)
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
            if (CallScope.IsInstanceMember && TypeQualifySymbol.HasContainQualify(Access.ReturnType, Root.Typeof))
            {
                CompileError("not-accessable");
            }
            if (CallScope.IsStaticMember && !TypeQualifySymbol.HasContainQualify(Access.ReturnType, Root.Typeof))
            {
                CompileError("not-accessable");
            }
            if (CallScope.IsStaticMember && !ContainClass(CallScope.GetParent<ClassSymbol>(), Access.ReturnType))
            {
                CompileError("undefined-identifier");
            }
        }

        private static bool ContainClass(ClassSymbol cls, Scope type)
        {
            if(type is ClassSymbol)
            {
                return cls == type;
            }
            else if(type is TypeQualifySymbol)
            {
                var tq = (TypeQualifySymbol)type;
                return cls == tq.BaseType;
            }
            else
            {
                return false;
            }
        }
    }
}
