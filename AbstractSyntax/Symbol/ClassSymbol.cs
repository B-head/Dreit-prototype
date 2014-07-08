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
        public DefaultSymbol Default { get; private set; }
        public ThisSymbol This { get; private set; }
        public bool IsClass { get; private set; }
        public bool IsTrait { get; private set; }
        protected IReadOnlyList<Scope> _Attribute;
        protected IReadOnlyList<ClassSymbol> _Inherit;
        protected IReadOnlyList<RoutineSymbol> _Initializer;

        protected ClassSymbol()
        {
            Default = new DefaultSymbol(this);
            This = new ThisSymbol(this);
            AppendChild(Default);
            AppendChild(This);
            _Attribute = new List<Scope>();
            _Inherit = new List<ClassSymbol>();
            _Initializer = new List<RoutineSymbol>();
        }

        protected ClassSymbol(TextPosition tp, string name, bool isTrait)
            :base(tp)
        {
            Name = name;
            IsClass = !isTrait;
            IsTrait = isTrait;
            Default = new DefaultSymbol(this);
            This = new ThisSymbol(this);
            AppendChild(Default);
            AppendChild(This);
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<ClassSymbol> Inherit
        {
            get { return _Inherit; }
        }

        public virtual IReadOnlyList<RoutineSymbol> Initializer
        {
            get { return _Initializer; }
        }

        public ClassSymbol InheritClass
        {
            get
            {
                var obj = NameResolution("Object").FindDataType() as ClassSymbol;
                if (this == obj)
                {
                    return null;
                }
                return _Inherit.FirstOrDefault(v => v.IsClass) ?? obj; 
            }
        }

        public IReadOnlyList<ClassSymbol> InheritTraits
        {
            get { return _Inherit.Where(v => v.IsTrait).ToList(); }
        }

        public RoutineSymbol DefaultInitializer
        {
            get { return Initializer.FirstOrDefault(v => v.ArgumentTypes.Count == 0); }
        }

        public bool IsPrimitive
        {
            get { return PrimitiveType != PrimitivePragmaType.NotPrimitive; }
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            foreach(var a in Initializer)
            {
                foreach (var b in a.GetTypeMatch(pars, args))
                {
                    yield return b;
                }
            }
        }

        internal override OverLoadReference NameResolution(string name)
        {
            if (ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            var n = CurrentScope.NameResolution(name);
            InitInherits i = () => InheritNameResolution(name);
            if (ChildSymbols.ContainsKey(name))
            {
                var s = ChildSymbols[name];
                n = new OverLoadReference(Root, n, i, s);
            }
            else
            {
                n = new OverLoadReference(Root, n, i);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        private IReadOnlyList<OverLoadReference> InheritNameResolution(string name)
        {
            var ret = new List<OverLoadReference>();
            foreach(var v in Inherit)
            {
                var ol = v.NameResolution(name);
                if(ol != null)
                {
                    ret.Add(ol);
                }
            }
            return ret;
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
