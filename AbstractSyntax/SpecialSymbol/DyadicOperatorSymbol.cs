/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class DyadicOperatorSymbol : RoutineSymbol
    {
        public TokenType CalculateType { get; private set; }

        public DyadicOperatorSymbol(TokenType type, TypeSymbol left, TypeSymbol right, TypeSymbol ret)
            : base(RoutineType.FunctionOperator, type)
        {
            Name = GetOperatorName(type);
            CalculateType = type;
            _Arguments = ArgumentSymbol.MakeParameters(left, right);
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
