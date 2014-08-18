using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class ErrorRoutineSymbol : RoutineSymbol
    {
        public ErrorRoutineSymbol()
            :base(RoutineType.Unknown, TokenType.Unknoun)
        {

        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {

        }
    }
}
