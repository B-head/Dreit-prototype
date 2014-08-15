using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class RoutineTemplateInstance : RoutineSymbol
    {
        public RoutineSymbol Routine { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }

        public RoutineTemplateInstance(RoutineSymbol routine, IReadOnlyList<TypeSymbol> parameter)
            :base(RoutineType.Unknown, TokenType.Unknoun)
        {
            Routine = routine;
            Parameters = parameter;
        }
    }
}
