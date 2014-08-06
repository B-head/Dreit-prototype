using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [Serializable]
    public class CastPragma : RoutineSymbol
    {
        public CastPragmaType PrimitiveType { get; private set; }

        public CastPragma(CastPragmaType type, ClassSymbol from, ClassSymbol to)
        {
            Name = to.Name;
            PrimitiveType = type;
            _ArgumentTypes = new Scope[] { from };
            _CallReturnType = to;
        }
    }

    public enum CastPragmaType
    {
        NotPrimitive = 0,
        Object = -1,
        String = -2,
        Boolean = -3,
        Integer8 = 1,
        Integer16 = 3,
        Integer32 = 5,
        Integer64 = 7,
        Natural8 = 2,
        Natural16 = 4,
        Natural32 = 6,
        Natural64 = 8,
        Binary32 = 9,
        Binary64 = 10,
    }
}
