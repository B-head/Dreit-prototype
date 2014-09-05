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

        public MonadicOperatorSymbol(TokenType type, TypeSymbol expt, TypeSymbol ret)
            :base(RoutineType.FunctionOperator, type)
        {
            Name = GetOperatorName(type);
            CalculateType = type;
            _Arguments = ArgumentSymbol.MakeParameters(expt);
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
