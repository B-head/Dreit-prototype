﻿using AbstractSyntax.SpecialSymbol;
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
        private RoutineSymbol _CallScope;

        public Prefix(TextPosition tp, TokenType op, Element exp)
            :base(tp, op, exp)
        {

        }

        public RoutineSymbol CallScope
        {
            get
            {
                if (_CallScope == null)
                {
                    _CallScope = Root.OpManager.FindMonadic(Operator, Exp.ReturnType);
                }
                return _CallScope;
            }
        }

        public override Scope ReturnType
        {
            get { return CallScope.CallReturnType; }
        }

        public override bool IsConstant
        {
            get { return Exp.IsConstant && CallScope.IsFunction; }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (CallScope is ErrorRoutineSymbol)
            {
                cmm.CompileError("undefined-monadic-operator", this);
            }
        }
    }
}
