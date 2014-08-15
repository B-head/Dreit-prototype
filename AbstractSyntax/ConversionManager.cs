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
    class ConversionManager
    {
        private Root Root;
        private List<RoutineSymbol> ConvList;

        public ConversionManager(Root root)
        {
            Root = root;
            ConvList = new List<RoutineSymbol>();
        }

        public void Append(RoutineSymbol symbol)
        {
            ConvList.Add(symbol);
        }

        public RoutineSymbol Find(TypeSymbol from, TypeSymbol to)
        {
            var s = ConvList.FindAll(v => v.CallReturnType == to && v.Arguments[0].ReturnType == from);
            if(s.Count == 1)
            {
                return s[0];
            }
            else if(s.Count > 1)
            {
                return Root.ErrorRoutine;
            }
            else
            {
                if (ContainSubType(from, to))
                {
                    return Root.Default;
                }
                else
                {
                    return Root.ErrorRoutine;
                }
            }
        }

        private static bool ContainSubType(TypeSymbol from, TypeSymbol to)
        {
            var f = from as ClassSymbol;
            if(f == null)
            {
                return from == to;
            }
            foreach(var v in f.EnumSubType())
            {
                if(v == to)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
