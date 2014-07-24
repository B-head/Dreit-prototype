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
        private OverLoadReference _Reference;

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

        public override OverLoadReference OverLoad
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

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (OverLoad.IsUndefined)
            {
                cmm.CompileError("undefined-identifier", this);
            }
            if (HasAnyAttribute(CallScope.Attribute, AttributeType.Private) && !HasCurrentAccess(CallScope.GetParent<ClassSymbol>()))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (HasAnyAttribute(CallScope.Attribute, AttributeType.Protected) && !HasCurrentAccess(CallScope.GetParent<ClassSymbol>()))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (CallScope.IsInstanceMember && QualifyTypeSymbol.HasContainQualify(Access.ReturnType, Root.Typeof))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (CallScope.IsStaticMember && !QualifyTypeSymbol.HasContainQualify(Access.ReturnType, Root.Typeof))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (CallScope.IsStaticMember && !ContainClass(CallScope.GetParent<ClassSymbol>(), Access.ReturnType))
            {
                cmm.CompileError("undefined-identifier", this);
            }
        }

        private static bool ContainClass(ClassSymbol cls, Scope type)
        {
            if(type is ClassSymbol)
            {
                return cls == type;
            }
            else if(type is QualifyTypeSymbol)
            {
                var tq = (QualifyTypeSymbol)type;
                return cls == tq.BaseType;
            }
            else
            {
                return false;
            }
        }
    }
}
