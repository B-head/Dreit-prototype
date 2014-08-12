using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class LambdaLiteral : RoutineDeclaration
    {
        public LambdaLiteral(TextPosition tp, string name, RoutineType type, TokenType op, TupleLiteral attr, TupleLiteral generic, TupleLiteral args, Element expli, ProgramContext block)
            : base(tp, name, type, op, attr, generic, args, expli, block)
        {
        }
    }
}
