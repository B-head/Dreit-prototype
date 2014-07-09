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
            OpList.Add(TokenType.Incompare, new List<RoutineSymbol>());
        }

        public void Append(RoutineSymbol symbol)
        {
            OpList[symbol.Operator].Add(symbol);
        }

        public Scope Find(TokenType op, Scope left, Scope right)
        {
            if (left is UnknownSymbol || right is UnknownSymbol || left is GenericSymbol || right is GenericSymbol)
            {
                return Root.Unknown;
            }
            var s = OpList[op].FindAll(v => v.CurrentScope == right && v.ArgumentTypes[0] == left);
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
