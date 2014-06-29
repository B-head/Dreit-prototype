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

        public abstract Element Access { get; }
        public abstract TupleList Arguments { get; }
        public abstract bool HasCallTarget(IElement element);
        public abstract IDataType GetCallType();

        public TypeMatch Match
        {
            get
            {
                if (_Match == null)
                {
                    var access = Access as IAccess;
                    if (access == null)
                    {
                        _Match = new TypeMatch();
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
                    return Arguments.GetDataTypes()[0];
                }
                else if (CallScope is RoutineSymbol)
                {
                    var rout = (RoutineSymbol)CallScope;
                    return rout.DataType;
                }
                else
                {
                    return CallScope.DataType; //todo もっと適切な方法で型を取得する必要がある。
                }
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
        }
    }
}
