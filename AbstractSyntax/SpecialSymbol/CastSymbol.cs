using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class CastSymbol : RoutineSymbol
    {
        public PrimitiveType PrimitiveType { get; private set; }

        public CastSymbol(PrimitiveType type, ClassSymbol from, ClassSymbol to)
            :base(TokenType.Unknoun, true)
        {
            Name = to.Name;
            PrimitiveType = type;
            _ArgumentTypes = new Scope[] { from };
            _CallReturnType = to;
        }
    }

    public enum PrimitiveType
    {
        NotPrimitive = 0,
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
