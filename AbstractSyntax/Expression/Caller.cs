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
    public abstract class Caller : Element
    {
        private TypeMatch? _Match;
        private Scope _CallScope;
        private Scope _CalculateCallScope;

        public abstract Element Access { get; }
        public abstract TupleList Arguments { get; }
        public abstract TokenType CalculateOperator { get; }
        public abstract bool HasCallTarget(IElement element);
        public abstract IDataType CallType { get; } //todo Tuple型も返せるようにする。

        public TypeMatch Match
        {
            get
            {
                if (_Match == null)
                {
                    _Match = Access.Reference.CallSelect(Arguments.GetDataTypes());
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

        public override IDataType ReturnType
        {
            get
            {
                if (CallScope is CalculatePragma || CallScope is CastPragma)
                {
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
