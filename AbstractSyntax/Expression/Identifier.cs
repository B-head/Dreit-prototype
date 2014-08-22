/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
        private OverLoadCallMatch? _Match;
        private OverLoad _OverLoad;

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
            get { return ReferVariant.VariantType == VariantType.Const; }
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
                if(_OverLoad != null)
                {
                    return _OverLoad;
                }
                if (IdentType == TokenType.Pragma)
                {
                    _OverLoad = CurrentScope.NameResolution("@@" + Value);
                }
                else if(IdentType == TokenType.Nullable)
                {
                    var type = CurrentScope.NameResolution(Value).FindDataType().Type;
                    var nullable = Root.ClassManager.Issue(Root.Nullable, new TypeSymbol[] { type }, new TypeSymbol[0]);
                    _OverLoad = nullable.OverLoad;
                }
                else
                {
                    _OverLoad = CurrentScope.NameResolution(Value);
                }
                return _OverLoad;
            }
        }

        public bool IsTacitThis
        {
            get
            {
                if (!AccessSymbol.IsInstanceMember || AccessSymbol is ThisSymbol)
                {
                    return false;
                }
                var pcls = AccessSymbol.DeclaringType;
                var cls = GetParent<ClassSymbol>();
                while (cls != null)
                {
                    if (cls == pcls)
                    {
                        return true;
                    }
                    cls = cls.InheritClass as ClassSymbol;
                }
                return false;
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

        //todo 重複コードをリファクタリングする。
        private bool IsConnectCalls 
        {
            get { return Parent is CallExpression || Parent is Postfix || Parent is TemplateInstanceExpression || Parent.Parent is TemplateInstanceExpression; }
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
            if(IsConnectCalls || !IsExecutionLocation)
            {
                return;
            }
            switch (Match.Result)
            {
                case CallMatchResult.NotCallable: cmm.CompileError("not-callable", this); break;
                case CallMatchResult.UnmatchArgumentCount: cmm.CompileError("unmatch-overload-count", this); break;
                case CallMatchResult.UnmatchArgumentType: cmm.CompileError("unmatch-overload-type", this); break;
                case CallMatchResult.UnmatchGenericCount: cmm.CompileError("unmatch-generic-count", this); break;
                case CallMatchResult.UnmatchGenericType: cmm.CompileError("unmatch-generic-type", this); break;
                case CallMatchResult.AmbiguityMatch: cmm.CompileError("ambiguity-match", this); break;
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
