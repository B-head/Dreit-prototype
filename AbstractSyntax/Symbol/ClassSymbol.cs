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
        public TypeofClassSymbol TypeofSymbol { get; private set; }
        protected List<RoutineSymbol> Initializer;
        protected List<IScope> _Attribute;
        protected List<ClassSymbol> _Inherit;

        public ClassSymbol()
        {
            TypeofSymbol = new TypeofClassSymbol(this);
            Initializer = new List<RoutineSymbol>();
        }

        public override IReadOnlyList<IScope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<ClassSymbol> Inherit
        {
            get { return _Inherit; }
        }

        public bool IsPrimitive
        {
            get { return GetPrimitiveType() != PrimitivePragmaType.NotPrimitive; }
        }

        public override int Count
        {
            get { return 1; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return TypeofSymbol;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            foreach(var a in Initializer)
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
    }
}
