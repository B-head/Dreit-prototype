using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class DyadicOperatorPragma : RoutineSymbol
    {
        public TokenType CalculateType { get; private set; }

        public DyadicOperatorPragma(TokenType type, ClassSymbol left, ClassSymbol right, ClassSymbol ret)
            : base(type)
        {
            Name = GetOperatorName(type);
            CalculateType = type;
            _ArgumentTypes = new Scope[] { left, right };
            _CallReturnType = ret;
        }

        internal static bool HasCondition(TokenType type)
        {
            switch(type)
            {
                case TokenType.Equal:
                case TokenType.NotEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    return true;
            }
            return false;
        }

        private static string GetOperatorName(TokenType type)
        {
            switch(type)
            {
                case TokenType.Add: return "+";
                case TokenType.Subtract: return "-";
                case TokenType.Multiply: return "*";
                case TokenType.Divide: return "/";
                case TokenType.Modulo: return "%";
                case TokenType.Equal: return "=";
                case TokenType.NotEqual: return "<>";
                case TokenType.LessThan: return "<";
                case TokenType.LessThanOrEqual: return "<=";
                case TokenType.GreaterThan: return ">";
                case TokenType.GreaterThanOrEqual: return ">=";
                default: throw new ArgumentException("type");
            }
        }

        public static IEnumerable<TokenType> EnumOperator()
        {
            yield return TokenType.Add;
            yield return TokenType.Subtract;
            yield return TokenType.Multiply;
            yield return TokenType.Divide;
            yield return TokenType.Modulo;
            yield return TokenType.Equal;
            yield return TokenType.NotEqual;
            yield return TokenType.LessThan;
            yield return TokenType.LessThanOrEqual;
            yield return TokenType.GreaterThan;
            yield return TokenType.GreaterThanOrEqual;
        }
    }
}
