using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class MonadicOperatorPragma : RoutineSymbol
    {
        public MonadicOperatorPragmaType CalculateType { get; private set; }

        public MonadicOperatorPragma(MonadicOperatorPragmaType type, ClassSymbol expt, ClassSymbol ret)
            :base(GetOperatorTokenType(type))
        {
            Name = GetOperatorName(type);
            CalculateType = type;
            _ArgumentTypes = new Scope[] { expt };
            _CallReturnType = ret;
        }

        internal static bool HasCondition(MonadicOperatorPragmaType type)
        {
            switch (type)
            {
                case MonadicOperatorPragmaType.Not:
                    return true;
            }
            return false;
        }

        private static string GetOperatorName(MonadicOperatorPragmaType type)
        {
            switch (type)
            {
                case MonadicOperatorPragmaType.Plus: return "++";
                case MonadicOperatorPragmaType.Minus: return "--";
                case MonadicOperatorPragmaType.Not: return "!!";
                default: throw new ArgumentException("type");
            }
        }

        private static TokenType GetOperatorTokenType(MonadicOperatorPragmaType type)
        {
            switch (type)
            {
                case MonadicOperatorPragmaType.Plus: return TokenType.Plus;
                case MonadicOperatorPragmaType.Minus: return TokenType.Minus;
                case MonadicOperatorPragmaType.Not: return TokenType.Not;
                default: throw new ArgumentException("type");
            }
        }
    }

    public enum MonadicOperatorPragmaType
    {
        Plus,
        Minus,
        Not,
    }
}
