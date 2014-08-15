using AbstractSyntax.Declaration;
using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Identifier : Element
    {
        public string Value { get; private set; }
        public TokenType IdentType { get; private set; }
        private bool? _IsTacitThis;
        private OverLoad _Reference;

        public Identifier(TextPosition tp, string value, TokenType identType)
            :base(tp)
        {
            Value = value;
            IdentType = identType;
        }

        protected override string ElementInfo
        {
            get
            {
                switch(IdentType)
                {
                    case TokenType.Pragma: return "@@" + Value;
                    case TokenType.Macro: return "##" + Value;
                    case TokenType.Nullable: return "??" + Value;
                    default: return Value;
                }
            }
        }

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
        }

        public override bool IsConstant
        {
            get { return CallRoutine.IsFunction; }
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
                if(_Reference != null)
                {
                    return _Reference;
                }
                if (IdentType == TokenType.Pragma)
                {
                    _Reference = CurrentScope.NameResolution("@@" + Value);
                }
                else
                {
                    _Reference = CurrentScope.NameResolution(Value);
                }
                return _Reference;
            }
        }

        public bool IsTacitThis
        {
            get
            {
                if (_IsTacitThis != null)
                {
                    return _IsTacitThis.Value;
                }
                _IsTacitThis = HasThisMember(AccessSymbol);
                return _IsTacitThis.Value;
            }
        }

        public ThisSymbol ThisReference
        {
            get
            {
                var c = GetParent<ClassSymbol>();
                return c.This;
            }
        }

        public RoutineSymbol ThisCallRoutine
        {
            get
            {
                var c = GetParent<ClassSymbol>();
                return c.This.OverLoad.CallSelect().Call;
            }
        }

        private bool IsStaticLocation()
        {
            var r = GetParent<RoutineSymbol>();
            if (r == null)
            {
                return false;
            }
            return r.IsStaticMember;
        }

        private bool HasThisMember(Scope scope)
        {
            if (scope.IsStaticMember || scope is ThisSymbol)
            {
                return false;
            }
            var cls = GetParent<ClassSymbol>();
            while (cls != null)
            {
                if (scope.GetParent<ClassSymbol>() == cls)
                {
                    return true;
                }
                cls = cls.InheritClass as ClassSymbol;
            }
            return false;
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (OverLoad.IsUndefined)
            {
                cmm.CompileError("undefined-identifier", this);
            }
            //todo より適切なエラーメッセージを出す。
            if (AccessSymbol.Attribute.HasAnyAttribute(AttributeType.Private) && !HasCurrentAccess(AccessSymbol.GetParent<ClassSymbol>()))
            {
                cmm.CompileError("not-accessable", this);
            }
            if (AccessSymbol.IsInstanceMember && IsStaticLocation() && !(Parent is Postfix)) //todo Postfixだけではなく包括的な例外処理をする。
            {
                cmm.CompileError("not-accessable", this);
            }
        }
    }
}
