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
using AbstractSyntax.Literal;
using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class CallExpression : Element
    {
        public Element Access { get; private set; }
        public TupleLiteral Arguments { get; private set; }
        private OverLoadCallMatch? _Match;
        private RoutineSymbol _CalculateCallScope;

        public CallExpression(TextPosition tp, Element acs, TupleLiteral args)
            : base(tp)
        {
            Access = acs;
            Arguments = args;
            AppendChild(Access);
            AppendChild(Arguments);
        }

        protected CallExpression(TextPosition tp, Element acs, Element arg)
            :base(tp)
        {
            Access = acs;
            if (arg is TupleLiteral)
            {
                Arguments = (TupleLiteral)arg;
            }
            else
            {
                Arguments = new TupleLiteral(arg.Position, arg);
            }
            AppendChild(Access);
            AppendChild(Arguments);
        }

        public virtual Element Left
        {
            get
            {
                return Access;
            }
        }

        public virtual Element Right
        {
            get
            {
                return (Element)Arguments[0];
            }
        }

        public IReadOnlyList<Element> VirtualArgument
        {
            get
            {
                if(IsConnectCall)
                {
                    var call = (CallExpression)Access;
                    return call.Arguments.Concat(Arguments).ToList();
                }
                else
                {
                    return Arguments;
                }
            }
        }

        public OverLoadCallMatch Match
        {
            get
            {
                if (_Match != null)
                {
                    return _Match.Value;
                }
                _Match = Access.OverLoad.CallSelect(VirtualArgument.GetDataTypes());
                return _Match.Value;
            }
        }

        public VariantSymbol ReferVariant
        {
            get { return Access.OverLoad.FindVariant(); }
        }

        public RoutineSymbol CallRoutine
        {
            get { return Match.Call; }
        }

        public IReadOnlyList<RoutineSymbol> CallConverter
        {
            get { return Match.Converters; }
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                return CallRoutine.CallReturnType;
            }
        }

        public override OverLoad OverLoad
        {
            get
            {
                if(IsConnectPipeline)
                {
                    return Access.OverLoad;
                }
                else
                {
                    return Root.SimplexManager.Issue(this);
                }
            }
        }

        public bool IsReferVeriant
        {
            get { return !(ReferVariant is ErrorVariantSymbol); }
        }

        public bool IsStoreCall
        {
            get { return RoutineSymbol.HasLoadStoreCall(CallRoutine); }
        }

        public override bool IsConstant
        {
            get 
            {
                if (IsCalculate && !CalculateCallScope.IsFunction)
                {
                    return false;
                }
                if (Access is VariantDeclaration)
                {
                    return IsDataTypeLocation && Arguments.IsConstant;
                }
                else
                {
                    return Access.IsConstant && Arguments.IsConstant && CallRoutine.IsFunction;
                }
            }
        }

        public bool IsCalculate
        {
            get { return CalculateOperator != TokenType.Unknoun; }
        }

        public bool IsFunctionLocation
        {
            get 
            {
                var f = GetParent<RoutineSymbol>();
                if(f == null)
                {
                    return false;
                }
                return f.IsFunction;
            }
        }

        public bool IsConstructorLocation
        {
            get
            {
                var f = GetParent<RoutineSymbol>();
                if (f == null)
                {
                    return false;
                }
                return f.IsConstructor;
            }
        }

        public bool IsDataTypeLocation
        {
            get { return CurrentScope is ClassDeclaration; }
        }

        public bool IsImmutableCall
        {
            get { return CallRoutine is PropertySymbol && ReferVariant.IsImmtable; }
        }

        public virtual bool IsPipeline
        {
            get { return false; }
        }

        public bool IsConnectCall
        {
            get { return IsPipeline && Access is CallExpression; }
        }

        public bool IsConnectPipeline
        {
            get { return !IsPipeline && (Parent is LeftPipeline || Parent is RightPipeline); }
        }

        public RoutineSymbol CalculateCallScope
        {
            get
            {
                if (_CalculateCallScope == null)
                {
                    if (IsCalculate)
                    {
                        _CalculateCallScope = Root.OpManager.FindDyadic(CalculateOperator, Left.ReturnType, Right.ReturnType);
                    }
                    else
                    {
                        _CalculateCallScope = Root.ErrorRoutine;
                    }
                }
                return _CalculateCallScope;
            }
        }

        public virtual TokenType CalculateOperator
        {
            get { return TokenType.Unknoun; }
        }

        public bool HasCallTarget(Element element)
        {
            return Access == element;
        }

        public TypeSymbol CallType //todo Tuple型も返せるようにする。
        {
            get
            {
                if (Arguments.Count != 1)
                {
                    return Root.ErrorType;
                }
                return Arguments[0].ReturnType;
            }
        }

        public Element CallValue
        {
            get
            {
                if (Arguments.Count != 1)
                {
                    return Root.ErrorVariant;
                }
                return Arguments[0];
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if(IsConnectPipeline)
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
            if (IsImmutableCall && !IsConstructorLocation && !(Access is VariantDeclaration))
            {
                cmm.CompileError("not-mutable", this);
            }
            //todo 副作用のある関数を呼べないようにする。
            if (IsFunctionLocation && CallRoutine is PropertySymbol)
            {
                cmm.CompileError("forbit-side-effect", this);
            }
            if (IsCalculate && CalculateCallScope is ErrorRoutineSymbol)
            {
                cmm.CompileError("impossible-calculate", this);
            }
        }
    }
}
