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
            OpList.Add(TokenType.Plus, new List<RoutineSymbol>());
            OpList.Add(TokenType.Minus, new List<RoutineSymbol>());
            OpList.Add(TokenType.Not, new List<RoutineSymbol>());
        }

        public void Append(RoutineSymbol symbol)
        {
            OpList[symbol.OperatorType].Add(symbol);
        }

        public Scope FindMonadic(TokenType op, Scope expt)
        {
            if (expt is UnknownSymbol || expt is GenericSymbol)
            {
                return Root.Unknown;
            }
            var s = OpList[op].FindAll(v => v.ArgumentTypes[0] == expt);
            if (s.Count == 1)
            {
                return s[0];
            }
            else
            {
                return Root.Error;
            }
        }

        public Scope FindDyadic(TokenType op, Scope left, Scope right)
        {
            if (left is UnknownSymbol || right is UnknownSymbol || left is GenericSymbol || right is GenericSymbol)
            {
                return Root.Unknown;
            }
            var s = OpList[op].FindAll(v => v.ArgumentTypes[0] == left && v.ArgumentTypes[1] == right);
            if (s.Count == 1)
            {
                return s[0];
            }
            else
            {
                return Root.Error;
            }
        }
    }
}
