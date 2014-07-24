using AbstractSyntax.Pragma;
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
        protected Dictionary<string, OverLoadReference> ReferenceCache { get; set; }

        protected Scope()
        {
            ChildSymbols = new Dictionary<string, OverLoadSet>();
            ReferenceCache = new Dictionary<string, OverLoadReference>();
        }

        protected Scope(TextPosition tp)
            : base(tp)
        {
            ChildSymbols = new Dictionary<string, OverLoadSet>();
            ReferenceCache = new Dictionary<string, OverLoadReference>();
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
                var ol = new OverLoadSet(CurrentScope);
                ChildSymbols.Add(scope.Name, ol);
            }
            ChildSymbols[scope.Name].Append(scope);
        }

        internal virtual OverLoadReference NameResolution(string name)
        {
            if(ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            var n = CurrentScope.NameResolution(name);
            if(ChildSymbols.ContainsKey(name))
            {
                var s = ChildSymbols[name];
                n = new OverLoadReference(Root, n, s);
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

        internal virtual IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            yield return TypeMatch.MakeNotCallable(Root.Unknown);
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
            get { return GetParent<ClassSymbol>() != null && HasAnyAttribute(Attribute, AttributeType.Static); }
        }

        public bool IsInstanceMember
        {
            get { return GetParent<ClassSymbol>() != null && !HasAnyAttribute(Attribute, AttributeType.Static); }
        }

        public bool IsThisCall
        {
            get 
            {
                var pp = this as PropertyPragma;
                if(pp == null)
                {
                    return false;
                }
                return pp.Variant is ThisSymbol; 
            }
        }
    }
}
