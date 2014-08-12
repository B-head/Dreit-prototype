using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    public enum ClassType
    {
        Unknown,
        Class,
        Trait,
        Extend,
    }

    [Serializable]
    public class ClassSymbol : Scope
    {
        public DefaultSymbol Default { get; private set; }
        public ThisSymbol This { get; private set; }
        public ClassType ClassType { get; private set; }
        public ProgramContext Block { get; private set; }
        protected IReadOnlyList<Scope> _Attribute;
        protected IReadOnlyList<GenericSymbol> _Generics;
        protected IReadOnlyList<Scope> _Inherit;
        private IReadOnlyList<RoutineSymbol> _Initializer;

        protected ClassSymbol()
        {
            Block = new ProgramContext();
            Default = new DefaultSymbol("new", this);
            This = new ThisSymbol(this);
            AppendChild(Block);
            AppendChild(Default);
            AppendChild(This);
            _Attribute = new List<Scope>();
            _Generics = new List<GenericSymbol>();
            _Inherit = new List<Scope>();
        }

        protected ClassSymbol(TextPosition tp, string name, ClassType type, ProgramContext block)
            :base(tp)
        {
            Name = name;
            ClassType = type;
            Block = block;
            Default = new DefaultSymbol("new", this);
            This = new ThisSymbol(this);
            AppendChild(Block);
            AppendChild(Default);
            AppendChild(This);
        }

        public ClassSymbol(string name, ClassType type, ProgramContext block, IReadOnlyList<Scope> attr, IReadOnlyList<GenericSymbol> gnr, IReadOnlyList<Scope> inherit)
        {
            Name = name;
            ClassType = type;
            Block = block;
            Default = new DefaultSymbol("new", this);
            This = new ThisSymbol(this);
            AppendChild(Block);
            AppendChild(Default);
            AppendChild(This);
            _Attribute = attr;
            _Generics = gnr;
            _Inherit = inherit;
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<GenericSymbol> Generics
        {
            get { return _Generics; }
        }

        public virtual IReadOnlyList<Scope> Inherit
        {
            get { return _Inherit; }
        }

        public IReadOnlyList<RoutineSymbol> Initializer
        {
            get
            {
                if (_Initializer != null)
                {
                    return _Initializer;
                }
                var i = new List<RoutineSymbol>();
                var newFlag = false;
                foreach (var e in Block)
                {
                    var r = e as RoutineSymbol;
                    if (r == null)
                    {
                        continue;
                    }
                    if (r.IsConstructor)
                    {
                        i.Add(r);
                        newFlag = true;
                    }
                    //else if (r.IsConvertor)
                    //{
                    //    Root.ConvManager.Append(r);
                    //    i.Add(r);
                    //}
                    //else if (r.Operator != TokenType.Unknoun)
                    //{
                    //    Root.OpManager.Append(r);
                    //}
                }
                if (!newFlag)
                {
                    i.Add(Default);
                }
                _Initializer = i;
                return _Initializer;
            }
        }

        public Scope InheritClass
        {
            get
            {
                var obj = NameResolution("Object").FindDataType() as ClassSymbol;
                if (this == obj)
                {
                    return null;
                }
                return Inherit.FirstOrDefault(v => !HasTrait(v)) ?? obj; 
            }
        }

        public IReadOnlyList<Scope> InheritTraits
        {
            get { return Inherit.Where(v => HasTrait(v)).ToList(); }
        }

        private bool HasTrait(Scope scope)
        {
            var c = scope as ClassSymbol;
            if(c != null)
            {
                return c.ClassType == ClassType.Trait;
            }
            return false;
        }

        public bool IsDefaultConstructor
        {
            get { return Initializer.Any(v => v is DefaultSymbol); }
        }

        public RoutineSymbol ZeroArgInitializer
        {
            get { return Initializer.FirstOrDefault(v => v.Arguments.Count == 0); }
        }

        public bool IsTrait
        {
            get { return ClassType == ClassType.Trait; }
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

        public IEnumerable<Scope> EnumSubType()
        {
            yield return this;
            foreach(var a in Inherit)
            {
                ClassSymbol c = a as ClassSymbol;
                foreach(var b in c.EnumSubType())
                {
                    yield return b;
                }
            }
        }
    }
}
