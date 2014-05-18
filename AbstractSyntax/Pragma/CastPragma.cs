using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CastPragma : RoutineSymbol, IPragma
    {
        //internal override TypeMatchResult GetTypeMatch(IReadOnlyList<DataType> type)
        //{
        //    if (type.Count != 2)
        //    {
        //        return TypeMatchResult.MissMatchCount;
        //    }
        //    else if (type[0] == type[1])
        //    {
        //        return TypeMatchResult.MissMatchType;
        //    }
        //    return TypeMatchResult.PerfectMatch;
        //}

        //todo ジェネリクスの構文で型検査をする。
        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<DataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, ArgumentType);
        }
    }
}
