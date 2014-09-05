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
using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class OperationManager
    {
        private Root Root;
        private Dictionary<TokenType, List<RoutineSymbol>> OpList;

        public OperationManager(Root root)
        {
            Root = root;
            OpList = new Dictionary<TokenType, List<RoutineSymbol>>();
            CreateLists();
        }

        private void CreateLists()
        {
            OpList.Add(TokenType.Add, new List<RoutineSymbol>());
            OpList.Add(TokenType.Subtract, new List<RoutineSymbol>());
            OpList.Add(TokenType.Join, new List<RoutineSymbol>());
            OpList.Add(TokenType.Multiply, new List<RoutineSymbol>());
            OpList.Add(TokenType.Divide, new List<RoutineSymbol>());
            OpList.Add(TokenType.Modulo, new List<RoutineSymbol>());
            OpList.Add(TokenType.Equal, new List<RoutineSymbol>());
            OpList.Add(TokenType.NotEqual, new List<RoutineSymbol>());
            OpList.Add(TokenType.LessThan, new List<RoutineSymbol>());
            OpList.Add(TokenType.LessThanOrEqual, new List<RoutineSymbol>());
            OpList.Add(TokenType.GreaterThan, new List<RoutineSymbol>());
            OpList.Add(TokenType.GreaterThanOrEqual, new List<RoutineSymbol>());
            OpList.Add(TokenType.Incomparable, new List<RoutineSymbol>());
            OpList.Add(TokenType.LeftCompose, new List<RoutineSymbol>());
            OpList.Add(TokenType.RightCompose, new List<RoutineSymbol>());
            OpList.Add(TokenType.Plus, new List<RoutineSymbol>());
            OpList.Add(TokenType.Minus, new List<RoutineSymbol>());
            OpList.Add(TokenType.Not, new List<RoutineSymbol>());
        }

        public void Append(RoutineSymbol symbol)
        {
            OpList[symbol.OperatorType].Add(symbol);
        }

        public RoutineSymbol FindMonadic(TokenType op, TypeSymbol expt)
        {
            var s = OpList[op].FindAll(v => v.Arguments[0].ReturnType == expt);
            if (s.Count == 1)
            {
                return s[0];
            }
            else
            {
                return Root.ErrorRoutine;
            }
        }

        public RoutineSymbol FindDyadic(TokenType op, TypeSymbol left, TypeSymbol right)
        {
            var s = OpList[op].FindAll(v => v.Arguments[0].ReturnType == left && v.Arguments[1].ReturnType == right);
            if (s.Count > 0)
            {
                return s[0];
            }
            else if(left == right)
            {
                return new DyadicOperatorSymbol(op, left, right, left);
            }
            else
            {
                return Root.ErrorRoutine;
            }
        }
    }
}
