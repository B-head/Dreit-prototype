using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CalculatePragma : RoutineSymbol, IPragma
    {
        public CalculatePragmaType Type { get; private set; }

        public CalculatePragma(CalculatePragmaType type)
        {
            Type = type;
        }

        //internal override TypeMatchResult GetTypeMatch(IReadOnlyList<DataType> type)
        //{
        //    if (type.Count != 2)
        //    {
        //        return TypeMatchResult.MissMatchCount;
        //    }
        //    else if (type[0] != type[1])
        //    {
        //        return TypeMatchResult.MissMatchType;
        //    }
        //    return TypeMatchResult.PerfectMatch;
        //}

        //todo これもジェネリクスいるねえ・・・
        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<DataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, ArgumentType);
        }
    }

    public enum CalculatePragmaType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
    }
}
