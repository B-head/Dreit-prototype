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
using AbstractSyntax.Literal;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class TemplateInstanceExpression : Element
    {
        public Element Access { get; private set; }
        public TupleLiteral DecParameters { get; private set; }
        private OverLoadCallMatch? _Match;
        private IReadOnlyList<TypeSymbol> _Parameter;

        public TemplateInstanceExpression(TextPosition tp, Element acs, TupleLiteral args)
            : base(tp)
        {
            Access = acs;
            DecParameters = args;
            AppendChild(Access);
            AppendChild(DecParameters);
        }

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
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
            get { return new OverLoadModify(Access.OverLoad, Parameter); }
        }

        public override bool IsConstant
        {
            get { return Access.IsConstant && ReferVariant.VariantType == VariantType.Const; }
        }

        public override dynamic GenerateConstantValue()
        {
            return ReferVariant.GenerateConstantValue();
        }

        private bool IsConnectCalls
        {
            get { return Parent is CallExpression || Parent is Postfix || Parent is TemplateInstanceExpression; }
        }

        private bool IsExecutionLocation
        {
            get { return CurrentScope.IsExecutionContext; }
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

        public IReadOnlyList<TypeSymbol> Parameter
        {
            get
            {
                if (_Parameter != null)
                {
                    return _Parameter;
                }
                var pt = new List<TypeSymbol>();
                foreach (var v in DecParameters)
                {
                    var temp = v.OverLoad.FindDataType().Type;
                    pt.Add(temp);
                }
                _Parameter = pt;
                return _Parameter;
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (OverLoad.IsUndefined)
            {
                cmm.CompileError("undefined-identifier", this);
            }
            if (IsConnectCalls || !IsExecutionLocation)
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
