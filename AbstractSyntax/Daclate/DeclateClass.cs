using AbstractSyntax.Expression;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateClass : ClassSymbol
    {
        public TupleList Generic { get; set; }
        public TupleList Inherit { get; set; }
        public DirectiveList Block { get; set; }
        public ThisSymbol This { get; set; }

        public override List<ClassSymbol> InheritRefer 
        {
            get
            {
                if (_InheritRefer != null)
                {
                    return _InheritRefer;
                }
                _InheritRefer = new List<ClassSymbol>();
                foreach (var v in Inherit)
                {
                    if (v.DataType is ClassSymbol)
                    {
                        InheritRefer.Add((ClassSymbol)v.DataType);
                    }
                    else
                    {
                        CompileError("not-datatype-inherit");
                    }
                }
                return _InheritRefer;
            }
        }

        public DeclateClass()
        {
            This = new ThisSymbol(this);
        }

        public override bool IsVoidValue
        {
            get { return true; } //todo この代わりのプロパティが必要。
        }

        public override DataType DataType
        {
            get { return this; }
        }

        public override int Count
        {
            get { return 4; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Generic;
                    case 1: return Inherit;
                    case 2: return Block;
                    case 3: return This;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal override TypeMatchResult TypeMatch(List<DataType> type)
        {
            if (type.Count == 0)
            {
                return TypeMatchResult.PerfectMatch;
            }
            else
            {
                return TypeMatchResult.MissMatchCount;
            }
        }

        public override PrimitivePragmaType GetPrimitiveType()
        {
            PrimitivePragma prim = null;
            if (InheritRefer.Count == 1)
            {
                prim = InheritRefer[0] as PrimitivePragma;
            }
            if(prim == null)
            {
                return PrimitivePragmaType.NotPrimitive;
            }
            else
            {
                return prim.Type;
            }
        }
    }
}
