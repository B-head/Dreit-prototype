using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ClassSymbol : DataType
    {
        protected List<ClassSymbol> _InheritRefer; 
        
        public virtual List<ClassSymbol> InheritRefer
        {
            get { return _InheritRefer; }
        }

        internal override TypeMatchResult TypeMatch(IReadOnlyList<DataType> type)
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
            if (prim == null)
            {
                return PrimitivePragmaType.NotPrimitive;
            }
            else
            {
                return prim.Type;
            }
        }

        public bool IsContain(ClassSymbol other)
        {
            return Object.ReferenceEquals(this, other);
        }

        public bool IsConvert(ClassSymbol other)
        {
            if (IsContain(other))
            {
                return true;
            }
            foreach (var v in InheritRefer)
            {
                if (v.IsConvert(other))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
