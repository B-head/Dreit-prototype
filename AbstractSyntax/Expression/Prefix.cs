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
    public class Prefix : MonadicExpression
    {
        private RoutineSymbol _CallRoutine;

        public Prefix(TextPosition tp, TokenType op, Element exp)
            :base(tp, op, exp)
        {

        }

        public RoutineSymbol CallRoutine
        {
            get
            {
                if (_CallRoutine == null)
                {
                    _CallRoutine = Root.OpManager.FindMonadic(Operator, Exp.ReturnType);
                }
                return _CallRoutine;
            }
        }

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
        }

        public override bool IsConstant
        {
            get { return Exp.IsConstant && CallRoutine.IsFunction; }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (CallRoutine is ErrorRoutineSymbol && !SyntaxUtility.HasAnyErrorType(Exp.ReturnType))
            {
                cmm.CompileError("undefined-monadic-operator", this);
            }
        }
    }
}
