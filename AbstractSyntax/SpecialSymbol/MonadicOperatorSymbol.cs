using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class MonadicOperatorSymbol : RoutineSymbol
    {
        public TokenType CalculateType { get; private set; }

        public MonadicOperatorSymbol(TokenType type, ClassSymbol expt, ClassSymbol ret)
            :base(type, true)
        {
            Name = GetOperatorName(type);
            CalculateType = type;
            _ArgumentTypes = new Scope[] { expt };
            _CallReturnType = ret;
        }

        internal static bool HasCondition(TokenType type)
        {
            switch (type)
            {
                case TokenType.Not:
                    return true;
            }
            return false;
        }

        private static string GetOperatorName(TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus: return "++";
                case TokenType.Minus: return "--";
                case TokenType.Not: return "!!";
                default: throw new ArgumentException("type");
            }
        }

        public static IEnumerable<TokenType> EnumOperator()
        {
            yield return TokenType.Plus;
            yield return TokenType.Minus;
            yield return TokenType.Not;
        }
    }
}
