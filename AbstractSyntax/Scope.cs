using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AbstractSyntax
{
    [Serializable]
    public abstract class Scope : Element
    {
        public string Name { get; protected set; }
        internal Dictionary<string, OverLoadSet> ChildSymbols { get; set; }
        protected Dictionary<string, OverLoadChain> ReferenceCache { get; set; }

        protected Scope()
        {
            ChildSymbols = new Dictionary<string, OverLoadSet>();
            ReferenceCache = new Dictionary<string, OverLoadChain>();
        }

        protected Scope(TextPosition tp)
            : base(tp)
        {
            ChildSymbols = new Dictionary<string, OverLoadSet>();
            ReferenceCache = new Dictionary<string, OverLoadChain>();
        }

        internal void SpreadChildScope(Element child)
        {
            var s = child as Scope;
            if (s != null)
            {
                AppendChildScope(s);
                return;
            }
            foreach (var v in child)
            {
                SpreadChildScope(v);
            }
        }

        internal void AppendChildScope(Scope scope)
        {
            if (string.IsNullOrEmpty(scope.Name))
            {
                return;
            }
            if (!ChildSymbols.ContainsKey(scope.Name))
            {
                var ol = new OverLoadSet(this);
                ChildSymbols.Add(scope.Name, ol);
            }
            ChildSymbols[scope.Name].Append(scope);
        }

        internal virtual OverLoadChain NameResolution(string name)
        {
            if(ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            var n = CurrentScope.NameResolution(name);
            if(ChildSymbols.ContainsKey(name))
            {
                var s = ChildSymbols[name];
                n = new OverLoadChain(this, n, s);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        public string FullName
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                BuildFullName(builder);
                return builder.ToString();
            }
        }

        private void BuildFullName(StringBuilder builder)
        {
            if(CurrentScope != null && !(CurrentScope is Root))
            {
                CurrentScope.BuildFullName(builder);
                builder.Append(".");
            }
            builder.Append(Name);
        }

        public Scope FindName(string name)
        {
            foreach(var v in this)
            {
                var s = v as Scope;
                if(s == null)
                {
                    continue;
                }
                if(s.Name == name)
                {
                    return s;
                }
            }
            return null;
        }

        protected override string ElementInfo
        {
            get { return Name; }
        }

        internal virtual IEnumerable<OverLoadMatch> GetTypeMatch(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            yield return OverLoadMatch.MakeNotCallable(Root.Unknown);
        }

        public virtual bool IsDataType
        {
            get { return false; }
        }

        public virtual Scope CallReturnType
        {
            get { return Root.Unknown; }
        }

        public virtual IReadOnlyList<Scope> Attribute
        {
            get { return new List<Scope>(); }
        }

        public bool IsStaticMember
        {
            get 
            {
                var rout = this as RoutineSymbol;
                if(rout != null && rout.IsConstructor)
                {
                    return false;
                }
                return GetParent<ClassSymbol>() != null && SyntaxUtility.HasAnyAttribute(Attribute, AttributeType.Static); 
            }
        }

        public bool IsInstanceMember
        {
            get
            {
                var rout = this as RoutineSymbol;
                if (rout != null && rout.IsConstructor)
                {
                    return false;
                }
                return GetParent<ClassSymbol>() != null && !SyntaxUtility.HasAnyAttribute(Attribute, AttributeType.Static); 
            }
        }

        public bool IsThisCall
        {
            get 
            {
                var pp = this as PropertySymbol;
                if(pp == null)
                {
                    return false;
                }
                return pp.Variant is ThisSymbol; 
            }
        }
    }
}
