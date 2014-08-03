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
        public DyadicOperatorPragmaType CalculateType { get; private set; }

        public DyadicOperatorPragma(DyadicOperatorPragmaType type, ClassSymbol left, ClassSymbol right, ClassSymbol ret)
            : base(GetOperatorTokenType(type))
        {
            Name = GetOperatorName(type);
            CalculateType = type;
            _ArgumentTypes = new Scope[] { left, right };
            _CallReturnType = ret;
        }

        internal static bool HasCondition(DyadicOperatorPragmaType type)
        {
            switch(type)
            {
                case DyadicOperatorPragmaType.EQ:
                case DyadicOperatorPragmaType.NE:
                case DyadicOperatorPragmaType.LT:
                case DyadicOperatorPragmaType.LE:
                case DyadicOperatorPragmaType.GT:
                case DyadicOperatorPragmaType.GE:
                    return true;
            }
            return false;
        }

        private static string GetOperatorName(DyadicOperatorPragmaType type)
        {
            switch(type)
            {
                case DyadicOperatorPragmaType.Add: return "+";
                case DyadicOperatorPragmaType.Sub: return "-";
                case DyadicOperatorPragmaType.Mul: return "*";
                case DyadicOperatorPragmaType.Div: return "/";
                case DyadicOperatorPragmaType.Mod: return "%";
                case DyadicOperatorPragmaType.EQ: return "=";
                case DyadicOperatorPragmaType.NE: return "<>";
                case DyadicOperatorPragmaType.LT: return "<";
                case DyadicOperatorPragmaType.LE: return "<=";
                case DyadicOperatorPragmaType.GT: return ">";
                case DyadicOperatorPragmaType.GE: return ">=";
                default: throw new ArgumentException("type");
            }
        }

        private static TokenType GetOperatorTokenType(DyadicOperatorPragmaType type)
        {
            switch (type)
            {
                case DyadicOperatorPragmaType.Add: return TokenType.Add;
                case DyadicOperatorPragmaType.Sub: return TokenType.Subtract;
                case DyadicOperatorPragmaType.Mul: return TokenType.Multiply;
                case DyadicOperatorPragmaType.Div: return TokenType.Divide;
                case DyadicOperatorPragmaType.Mod: return TokenType.Modulo;
                case DyadicOperatorPragmaType.EQ: return TokenType.Equal;
                case DyadicOperatorPragmaType.NE: return TokenType.NotEqual;
                case DyadicOperatorPragmaType.LT: return TokenType.LessThan;
                case DyadicOperatorPragmaType.LE: return TokenType.LessThanOrEqual;
                case DyadicOperatorPragmaType.GT: return TokenType.GreaterThan;
                case DyadicOperatorPragmaType.GE: return TokenType.GreaterThanOrEqual;
                default: throw new ArgumentException("type");
            }
        }
    }

    public enum DyadicOperatorPragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        EQ,
        NE,
        LT,
        LE,
        GT,
        GE,
    }
}
