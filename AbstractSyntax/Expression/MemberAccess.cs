using AbstractSyntax.SpecialSymbol;
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

        public override bool IsConstant
        {
            get { return Access.IsConstant && CallScope.IsFunction; }
        }

        public RoutineSymbol CallScope
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

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            //todo より適切なエラーメッセージを出す。
            if (OverLoad.IsUndefined)
            {
                cmm.CompileError("undefined-identifier", this);
            }
            if (CallScope.IsStaticMember && !ContainClass(CallScope.GetParent<ClassSymbol>(), Access.ReturnType))
            {
                cmm.CompileError("undefined-identifier", this);
            }
            if (SyntaxUtility.HasAnyAttribute(CallScope.Attribute, AttributeType.Private) && !HasCurrentAccess(CallScope.GetParent<ClassSymbol>()))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (SyntaxUtility.HasAnyAttribute(CallScope.Attribute, AttributeType.Protected) && !HasCurrentAccess(CallScope.GetParent<ClassSymbol>()))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (CallScope.IsInstanceMember && ModifyTypeSymbol.HasContainModify(Access.ReturnType, ModifyType.Typeof))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (CallScope.IsStaticMember && !ModifyTypeSymbol.HasContainModify(Access.ReturnType, ModifyType.Typeof))
            {
                cmm.CompileError("not-accessable", this);
            }
        }

        private static bool ContainClass(ClassSymbol cls, Scope type)
        {
            if(type is ClassSymbol)
            {
                return cls == type;
            }
            else if(type is TemplateInstanceSymbol)
            {
                var tis = (TemplateInstanceSymbol)type;
                return tis.ContainClass(cls);
            }
            else
            {
                return false;
            }
        }
    }
}
