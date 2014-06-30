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
        public abstract IDataType GetCallType(); //todo Tuple型も返せるようにする。

        public TypeMatch Match
        {
            get
            {
                if (_Match == null)
                {
                    var access = Access as IAccess;
                    if (access == null)
                    {
                        _Match = TypeMatch.MakeNotCallable(Root.Unknown);
                    }
                    else
                    {
                        _Match = access.Reference.TypeSelect(Arguments.GetDataTypes());
                    }
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

        public override IDataType DataType
        {
            get
            {
                if (CallScope is CalculatePragma || CallScope is CastPragma)
                {
                    return Arguments.GetDataTypes()[0]; //todo ジェネリクスを使用して型を返すようにする。
                }
                else
                {
                    return CallScope.ReturnType;
                }
            }
        }

        public bool IsCalculate
        {
            get { return CalculateOperator != TokenType.Unknoun; }
        }

        public Scope CalculateCallScope
        {
            get
            {
                if (_CalculateCallScope == null)
                {
                    if (IsCalculate)
                    {
                        _CalculateCallScope = Root.OpManager[CalculateOperator].Find(Arguments[0].DataType, Access.DataType);
                    }
                    else
                    {
                        _CalculateCallScope = Root.Unknown;
                    }
                }
                return _CalculateCallScope;
            }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            switch (Match.Result)
            {
                case TypeMatchResult.NotCallable: CompileError("not-callable"); break;
                case TypeMatchResult.UnMatchCount: CompileError("unmatch-overload-count"); break;
                case TypeMatchResult.UnMatchType: CompileError("unmatch-overload-type"); break;
            }
            if (CalculateCallScope is ErrorSymbol)
            {
                CompileError("impossible-calculate");
            }
        }
    }
}
