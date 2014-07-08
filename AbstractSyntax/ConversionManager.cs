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

        public Scope Find(Scope from, Scope to)
        {
            if (from is UnknownSymbol || to is UnknownSymbol || from is GenericSymbol || to is GenericSymbol)
            {
                return Root.Unknown;
            }
            var a = ConvList.FindAll(v => v.CurrentScope == to);
            var b = a.FindAll(v => v.ArgumentTypes[0] == from);
            if(b.Count > 0)
            {
                return b[0];
            }
            else
            {
                if (ContainSubType(from, to))
                {
                    return Root.Void;
                }
                else
                {
                    return Root.Error;
                }
            }
        }

        private static bool ContainSubType(Scope from, Scope to)
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
