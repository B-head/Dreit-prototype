using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    class ConversionManager
    {
        private List<RoutineSymbol> ConvList;
        private UndefinedSymbol Undefined;

        public ConversionManager(UndefinedSymbol undefined)
        {
            ConvList = new List<RoutineSymbol>();
            Undefined = undefined;
        }

        public void Append(RoutineSymbol symbol)
        {
            ConvList.Add(symbol);
        }

        public Scope Find(DataType from, DataType to)
        {
            var a = ConvList.FindAll(v => v.CurrentScope == to);
            var b = a.FindAll(v => v.ArgumentType[0] == from);
            if(b.Count > 0)
            {
                return b[0];
            }
            else
            {
                return Undefined;
            }
        }
    }
}
