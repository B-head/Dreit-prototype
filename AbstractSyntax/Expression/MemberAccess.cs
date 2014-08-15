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

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
        }

        public override bool IsConstant
        {
            get { return Access.IsConstant && CallRoutine.IsFunction; }
        }

        public VariantSymbol ReferVariant
        {
            get { return OverLoad.FindVariant(); }
        }

        public RoutineSymbol CallRoutine
        {
            get { return OverLoad.CallSelect().Call; }
        }

        public Scope AccessSymbol
        {
            get { return CallRoutine.IsAliasCall ? (Scope)ReferVariant : (Scope)CallRoutine; }
        }

        public override OverLoad OverLoad
        {
            get
            {
                if(_Reference == null)
                {
                    var scope = Access.ReturnType;
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
            if (AccessSymbol.IsStaticMember && !ContainClass(AccessSymbol.GetParent<ClassSymbol>(), Access.ReturnType))
            {
                cmm.CompileError("undefined-identifier", this);
            }
            if (AccessSymbol.Attribute.HasAnyAttribute(AttributeType.Private) && !HasCurrentAccess(AccessSymbol.GetParent<ClassSymbol>()))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (AccessSymbol.Attribute.HasAnyAttribute(AttributeType.Protected) && !HasCurrentAccess(AccessSymbol.GetParent<ClassSymbol>()))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (AccessSymbol.IsInstanceMember && ModifyTypeSymbol.HasContainModify(Access.ReturnType, ModifyType.Typeof))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (AccessSymbol.IsStaticMember && !ModifyTypeSymbol.HasContainModify(Access.ReturnType, ModifyType.Typeof))
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
            else if(type is ClassTemplateInstance)
            {
                var tis = (ClassTemplateInstance)type;
                return tis.ContainClass(cls);
            }
            else
            {
                return false;
            }
        }
    }
}
