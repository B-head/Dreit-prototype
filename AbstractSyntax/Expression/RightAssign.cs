using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class RightAssign : DyadicExpression, ICaller
    {
        private Scope _ConversionRoutine;
        private Scope _CallScope;
        public TupleList Arguments { get; set; }

        public Scope CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    var access = Right as IAccess;
                    if (access == null)
                    {
                        _CallScope = Root.Unknown;
                    }
                    else
                    {
                        _CallScope = access.Reference.TypeSelect(Arguments.GetDataTypes());
                    }
                }
                return _CallScope;
            }
        }

        public override DataType DataType
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

        public Scope ConversionRoutine
        {
            get
            {
                if (_ConversionRoutine == null)
                {
                    _ConversionRoutine = Root.Conversion.Find(Left.DataType, Right.DataType);
                }
                return _ConversionRoutine;
            }
        }

        protected override void SpreadElement(Element parent, Scope scope)
        {
            if (Left is TupleList)
            {
                Arguments = (TupleList)Left;
            }
            else
            {
                Arguments = new TupleList(Left);
            }
            base.SpreadElement(parent, scope);
        }

        internal override void CheckSyntax()
        {
            if (!(Right is IAccess))
            {
                CompileError("not-assignable");
            }
            base.CheckSyntax();
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if (CallScope == null)
            {
                CompileError("unmatch-overroad");
            }
            if (Left.DataType != Right.DataType && ConversionRoutine is VoidSymbol)
            {
                CompileError("not-convertable-right");
            }
        }

        public DataType GetCallType()
        {
            DeclateVariant temp = Right as DeclateVariant;
            if (temp == null)
            {
                return Root.Unknown;
            }
            return Left.DataType;
        }
    }
}
