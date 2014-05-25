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
    public class ClassSymbol : Scope, IDataType
    {
        protected List<RoutineSymbol> initializer;
        protected List<ClassSymbol> _InheritRefer;

        public ClassSymbol()
        {
            initializer = new List<RoutineSymbol>();
        }
        
        public virtual List<ClassSymbol> InheritRefer
        {
            get { return _InheritRefer; }
        }

        public override IDataType DataType
        {
            get { return this; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            foreach(var a in initializer)
            {
                foreach(var b in a.GetTypeMatch(type))
                {
                    yield return b;
                }
            }
        }

        public PrimitivePragmaType GetPrimitiveType()
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
