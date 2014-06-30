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
    public class ClassSymbol : Scope, IDataType, IAttribute
    {
        protected List<RoutineSymbol> initializer;
        protected List<IScope> _Attribute;
        protected List<ClassSymbol> _Inherit;

        public ClassSymbol()
        {
            initializer = new List<RoutineSymbol>();
        }

        public virtual IReadOnlyList<IScope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<ClassSymbol> Inherit
        {
            get { return _Inherit; }
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
            if (Inherit.Count == 1)
            {
                prim = Inherit[0] as PrimitivePragma;
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
            foreach (var v in Inherit)
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
