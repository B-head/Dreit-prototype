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
        private OverLoadMatch? _Match;
        private RoutineSymbol _CallRoutine;
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

        public OverLoadMatch Match
        {
            get
            {
                if (_Match != null)
                {
                    return _Match.Value;
                }
                _Match = Access.OverLoad.CallSelect(Arguments.GetDataTypes());
                return _Match.Value;
            }
        }

        public VariantSymbol ReferVariant
        {
            get { return Access.OverLoad.FindVariant(); }
        }

        public RoutineSymbol CallRoutine
        {
            get
            {
                if (_CallRoutine == null)
                {
                    _CallRoutine = Match.Call;
                }
                return _CallRoutine;
            }
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                return CallRoutine.CallReturnType;
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

        public bool IsDataTypeLocation
        {
            get { return CurrentScope is ClassDeclaration; }
        }

        public bool IsImmutableCall
        {
            get { return ReferVariant.IsImmtable; }
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
                if (Arguments.GetDataTypes().Count != 1)
                {
                    return Root.ErrorType;
                }
                return Arguments.GetDataTypes()[0];
            }
        }

        public object GenelateConstantValue()
        {
            if (Arguments.Count != 1)
            {
                return null;
            }
            var exp = Arguments[0] as NumericLiteral;
            if(exp == null)
            {
                return null;
            }
            return exp.Parse();
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            switch (Match.Result)
            {
                case TypeMatchResult.NotCallable: cmm.CompileError("not-callable", this); break;
                case TypeMatchResult.UnmatchArgumentCount: cmm.CompileError("unmatch-overload-count", this); break;
                case TypeMatchResult.UnmatchArgumentType: cmm.CompileError("unmatch-overload-type", this); break;
                case TypeMatchResult.UnmatchGenericCount: cmm.CompileError("unmatch-generic-count", this); break;
                case TypeMatchResult.UnmatchGenericType: cmm.CompileError("unmatch-generic-type", this); break;
                case TypeMatchResult.AmbiguityMatch: cmm.CompileError("ambiguity-match", this); break;
            }
            if (IsImmutableCall && !(Access is VariantDeclaration))
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
