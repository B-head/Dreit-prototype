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
        private OverLoadCallMatch? _Match;
        private OverLoad _OverLoad;

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
            get { return Access.IsConstant && ReferVariant.VariantType == VariantType.Const; }
        }

        public override dynamic GenerateConstantValue()
        {
            return ReferVariant.GenerateConstantValue();
        }

        public VariantSymbol ReferVariant
        {
            get { return OverLoad.FindVariant(); }
        }

        public RoutineSymbol CallRoutine
        {
            get { return Match.Call; }
        }

        public Scope AccessSymbol
        {
            get { return CallRoutine.IsAliasCall ? (Scope)ReferVariant : (Scope)CallRoutine; }
        }

        public OverLoadCallMatch Match
        {
            get
            {
                if (_Match != null)
                {
                    return _Match.Value;
                }
                _Match = OverLoad.CallSelect();
                return _Match.Value;
            }
        }

        public override OverLoad OverLoad
        {
            get
            {
                if(_OverLoad == null)
                {
                    _OverLoad = Access.ReturnType.NameResolution(Member);
                }
                return _OverLoad;
            }
        }

        private bool IsConnectCalls
        {
            get { return Parent is CallExpression || Parent is Postfix || Parent is TemplateInstanceExpression; }
        }

        private bool IsExecutionLocation
        {
            get { return CurrentScope.IsExecutionContext; }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (OverLoad.IsUndefined)
            {
                cmm.CompileError("undefined-identifier", this);
                return;
            }
            if (AccessSymbol.IsStaticMember && !ContainClass(AccessSymbol.GetParent<ClassSymbol>(), Access.ReturnType))
            {
                cmm.CompileError("undefined-identifier", this);
                return;
            }
            if (IsConnectCalls || !IsExecutionLocation)
            {
                return;
            }
            //todo より適切なエラーメッセージを出す。
            switch (Match.Result)
            {
                case CallMatchResult.NotCallable: cmm.CompileError("not-callable", this); break;
                case CallMatchResult.UnmatchArgumentCount: cmm.CompileError("unmatch-overload-count", this); break;
                case CallMatchResult.UnmatchArgumentType: cmm.CompileError("unmatch-overload-type", this); break;
                case CallMatchResult.UnmatchGenericCount: cmm.CompileError("unmatch-generic-count", this); break;
                case CallMatchResult.UnmatchGenericType: cmm.CompileError("unmatch-generic-type", this); break;
                case CallMatchResult.AmbiguityMatch: cmm.CompileError("ambiguity-match", this); break;
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
            if(type is ClassTemplateInstance)
            {
                var tis = (ClassTemplateInstance)type;
                return tis.ContainClass(cls);
            }
            else if(type is ClassSymbol)
            {
                return cls == type;
            }
            else
            {
                return false;
            }
        }
    }
}
