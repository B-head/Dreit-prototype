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
    public class DeclateClass : DataType
    {
        public TupleList Generic { get; set; }
        public TupleList Inherit { get; set; }
        public DirectiveList Block { get; set; }
        public ThisSymbol This { get; set; }
        public List<DeclateClass> InheritRefer { get; private set; }

        public DeclateClass()
        {
            InheritRefer = new List<DeclateClass>();
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

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            if(Inherit == null)
            {
                return;
            }
            foreach (var v in Inherit)
            {
                if(v.DataType is DeclateClass)
                {
                    InheritRefer.Add((DeclateClass)v.DataType);
                }
                else
                {
                    CompileError("not-datatype-inherit");
                }
            }
        }

        public bool IsContain(DeclateClass other)
        {
            return Object.ReferenceEquals(this, other);
        }

        public bool IsConvert(DeclateClass other)
        {
            if(IsContain(other))
            {
                return true;
            }
            foreach(var v in InheritRefer)
            {
                if(v.IsConvert(other))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
