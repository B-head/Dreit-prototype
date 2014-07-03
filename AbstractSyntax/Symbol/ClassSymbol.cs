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
        public ThisSymbol This { get; private set; }
        public bool IsClass { get; protected set; }
        public bool IsTrait { get; protected set; }
        protected List<RoutineSymbol> Initializer;
        protected List<IScope> _Attribute;
        protected List<ClassSymbol> _Inherit;

        public ClassSymbol()
        {
            TypeofSymbol = new TypeofClassSymbol(this);
            This = new ThisSymbol(this);
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

        public ClassSymbol InheritClass
        {
            get { return _Inherit.Find(v => v.IsClass); }
        }

        public IReadOnlyList<ClassSymbol> InheritTraits
        {
            get { return _Inherit.Where(v => v.IsTrait).ToList(); }
        }

        public RoutineSymbol DefaultInitializer
        {
            get { return Initializer.Find(v => v.ArgumentType.Count == 0); }
        }

        public bool IsPrimitive
        {
            get { return PrimitiveType != PrimitivePragmaType.NotPrimitive; }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return TypeofSymbol;
                    case 1: return This;
                    default: throw new ArgumentOutOfRangeException("index");
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

        internal OverLoad InheritNameResolution(string name)
        {
            var ol = GetOverLoad(name);
            if(!ol.IsUndefined)
            {
                return ol;
            }
            foreach(var v in Inherit)
            {
                ol = v.InheritNameResolution(name);
                if (!ol.IsUndefined)
                {
                    return ol;
                }
            }
            return Root.UndefinedOverLord;
        }

        public PrimitivePragmaType PrimitiveType
        {
            get
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
                    return prim.BasePrimitiveType;
                }
            }
        }
    }
}
