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
        private DeclateClass _DataType;

        public ThisSymbol(DeclateClass dataType)
        {
            Name = "this";
            _DataType = dataType;
        }

        public override DataType DataType
        {
            get { return _DataType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<DataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new DataType[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new DataType[] { DataType });
        }
    }
}
