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

        public override IDataType DataType
        {
            get { return _DataType; }
        }

        public override IDataType ReturnType
        {
            get { return DataType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { DataType });
        }
    }
}
