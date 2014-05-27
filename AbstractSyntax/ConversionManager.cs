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
        private VoidSymbol Void;
        private ErrorSymbol Error;
        private UnknownSymbol Unknown;

        public ConversionManager(VoidSymbol voidSym, ErrorSymbol error, UnknownSymbol unknown)
        {
            ConvList = new List<RoutineSymbol>();
            Void = voidSym;
            Error = error;
            Unknown = unknown;
        }

        public void Append(RoutineSymbol symbol)
        {
            ConvList.Add(symbol);
        }

        public Scope Find(IDataType from, IDataType to)
        {
            if (from is GenericSymbol || to is GenericSymbol)
            {
                return Void;
            }
            if (from is UnknownSymbol || to is UnknownSymbol)
            {
                return Unknown;
            }
            var a = ConvList.FindAll(v => v.CurrentScope == to);
            var b = a.FindAll(v => v.ArgumentType[0] == from);
            if(b.Count > 0)
            {
                return b[0];
            }
            else
            {
                if (from == to) //todo 応急処置的な。
                {
                    return Void;
                }
                else
                {
                    return Error;
                }
            }
        }
    }
}
