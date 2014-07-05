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
    public class ClassSymbol : Scope
    {
        public TypeofClassSymbol TypeofSymbol { get; private set; }
        public DefaultSymbol Default { get; private set; }
        public ThisSymbol This { get; private set; }
        public bool IsClass { get; private set; }
        public bool IsTrait { get; private set; }
        protected List<RoutineSymbol> Initializer;
        protected List<Scope> _Attribute;
        protected List<ClassSymbol> _Inherit;

        protected ClassSymbol()
        {
            TypeofSymbol = new TypeofClassSymbol(this);
            Default = new DefaultSymbol(this);
            This = new ThisSymbol(this);
            Initializer = new List<RoutineSymbol>();
        }

        protected ClassSymbol(TextPosition tp, string name, bool isTrait)
            :base(tp)
        {
            Name = name;
            IsClass = !isTrait;
            IsTrait = isTrait;
            TypeofSymbol = new TypeofClassSymbol(this);
            Default = new DefaultSymbol(this);
            This = new ThisSymbol(this);
            Initializer = new List<RoutineSymbol>();
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<ClassSymbol> Inherit
        {
            get { return _Inherit; }
        }

        public ClassSymbol InheritClass
        {
            get
            {
                if (this == Root.ObjectSymbol)
                {
                    return null;
                }
                return _Inherit.Find(v => v.IsClass) ?? Root.ObjectSymbol as ClassSymbol; 
            }
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

        public override Element this[int index]
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

        public override bool IsDataType
        {
            get { return true; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
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
                var prim = (PrimitivePragma)Inherit.FirstOrDefault(v => v is PrimitivePragma);
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

        public IEnumerable<ClassSymbol> EnumSubType()
        {
            yield return this;
            foreach(var a in Inherit)
            {
                foreach(var b in a.EnumSubType())
                {
                    yield return b;
                }
            }
        }
    }
}
