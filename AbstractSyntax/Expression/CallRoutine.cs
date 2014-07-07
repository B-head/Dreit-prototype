using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class CallRoutine : Element
    {
        public Element Access { get; private set; }
        public TupleList Arguments { get; private set; }
        private TypeMatch? _Match;
        private Scope _CallScope;
        private Scope _CalculateCallScope;

        public CallRoutine(TextPosition tp, Element acs, TupleList args)
            : base(tp)
        {
            Access = acs;
            Arguments = args;
            AppendChild(Access);
            AppendChild(Arguments);
        }

        protected CallRoutine(TextPosition tp, Element acs, Element arg)
            :base(tp)
        {
            Access = acs;
            if (arg is TupleList)
            {
                Arguments = (TupleList)arg;
            }
            else
            {
                Arguments = new TupleList(arg);
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
                if (_Match == null)
                {
                    _Match = Access.OverLoad.CallSelect(Arguments.GetDataTypes());
                }
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
                if (CallScope is CalculatePragma || CallScope is CastPragma)
                {
                    var cal = CallScope as CalculatePragma;
                    if(cal != null && cal.IsCondition)
                    {
                        return cal.BooleanSymbol;
                    }
                    return Arguments.GetDataTypes()[0]; //todo ジェネリクスを使用して型を返すようにする。
                }
                else
                {
                    return CallScope.CallReturnType;
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
                return f.IsAnyAttribute(AttributeType.Function);
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
                        _CalculateCallScope = Root.OpManager[CalculateOperator].Find(Arguments[0].ReturnType, Access.ReturnType);
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

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
            switch (Match.Result)
            {
                case TypeMatchResult.NotCallable: CompileError("not-callable"); break;
                case TypeMatchResult.UnmatchCount: CompileError("unmatch-overload-count"); break;
                case TypeMatchResult.UnmatchType: CompileError("unmatch-overload-type"); break;
            }
            if (CallScope.IsAnyAttribute(AttributeType.Let) && !(Access is DeclateVariant))
            {
                CompileError("not-mutable");
            }
            if (IsFunctionLocation && CallScope is VariantSymbol)
            {
                CompileError("forbit-side-effect");
            }
            if (CalculateCallScope is ErrorSymbol)
            {
                CompileError("impossible-calculate");
            }
        }
    }
}
