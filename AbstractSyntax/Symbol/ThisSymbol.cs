using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ThisSymbol : Scope
    {
        private ClassSymbol _DataType;

        public ThisSymbol(ClassSymbol dataType)
        {
            Name = "this";
            _DataType = dataType;
        }

        public override Scope ReturnType
        {
            get { return _DataType; }
        }

        public override Scope CallReturnType
        {
            get { return ReturnType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { ReturnType });
        }
    }
}
