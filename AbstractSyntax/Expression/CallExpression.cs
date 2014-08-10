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
        private TypeMatch? _Match;
        private Scope _CallScope;
        private Scope _CalculateCallScope;

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
                Arguments = new TupleLiteral(arg);
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

        public TypeMatch Match
        {
            get
            {
                if (_Match != null)
                {
                    return _Match.Value;
                }
                var tie = Access as TemplateInstance;
                IReadOnlyList<Scope> pars;
                if(tie == null)
                {
                    pars = new List<Scope>();
                }
                else
                {
                    pars = tie.Parameter;
                }
                _Match = Access.OverLoad.CallSelect(pars, Arguments.GetDataTypes());
                return _Match.Value;
            }
        }

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    _CallScope = Match.Call;
                }
                return _CallScope;
            }
        }

        public override Scope ReturnType
        {
            get
            {
                return CallScope.CallReturnType;
            }
        }

        public override bool IsConstant
        {
            get 
            {
                if (IsCalculate && !((RoutineSymbol)CalculateCallScope).IsFunction)
                {
                    return false;
                }
                if (Access is VariantDeclaration)
                {
                    return IsDataTypeLocation && Arguments.IsConstant;
                }
                else
                {
                    return Access.IsConstant && Arguments.IsConstant && ((RoutineSymbol)CallScope).IsFunction;
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
            get
            {
                var v = CallScope as PropertySymbol;
                if(v == null)
                {
                    return false;
                }
                return v.Variant.IsLet;
            }
        }

        public Scope CalculateCallScope
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
                        _CalculateCallScope = Root.Unknown;
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

        public Scope CallType //todo Tuple型も返せるようにする。
        {
            get
            {
                if (Arguments.GetDataTypes().Count != 1)
                {
                    return Root.Unknown;
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
            }
            if (IsImmutableCall && !(Access is VariantDeclaration))
            {
                cmm.CompileError("not-mutable", this);
            }
            if (IsFunctionLocation && CallScope is PropertySymbol)
            {
                cmm.CompileError("forbit-side-effect", this);
            }
            if (CalculateCallScope is ErrorSymbol)
            {
                cmm.CompileError("impossible-calculate", this);
            }
        }
    }
}
