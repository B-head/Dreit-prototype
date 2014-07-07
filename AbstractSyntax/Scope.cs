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
        private Dictionary<string, OverLoad> ScopeSymbol;

        protected Scope()
        {
            ScopeSymbol = new Dictionary<string, OverLoad>();
        }

        protected Scope(TextPosition tp)
            : base(tp)
        {
            ScopeSymbol = new Dictionary<string, OverLoad>();
        }

        protected void Merge(Scope other)
        {
            if(other == null)
            {
                throw new ArgumentNullException("other");
            }
            foreach (var v in other.ScopeSymbol)
            {
                if (!ScopeSymbol.ContainsKey(v.Key))
                {
                    ScopeSymbol.Add(v.Key, new OverLoad(this));
                }
                ScopeSymbol[v.Key].Merge(v.Value);
            }
        }

        internal OverLoad NameResolution(string name)
        {
            var cls = this as ClassSymbol;
            if (cls != null)
            {
                var ol = cls.InheritNameResolution(name);
                if(!ol.IsUndefined)
                {
                    return ol;
                }
            }
            else
            {
                var ol = GetOverLoad(name);
                if (!ol.IsUndefined)
                {
                    return ol;
                }
            }
            if (this is Root)
            {
                return Root.UndefinedOverLord;
            }
            return CurrentScope.NameResolution(name);
        }

        protected OverLoad GetOverLoad(string name)
        {
            if (!ScopeSymbol.ContainsKey(name))
            {
                return Root.UndefinedOverLord;
            }
            return ScopeSymbol[name];
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

        internal void SpreadChildScope(Element child)
        {
            var s = child as Scope;
            if(s != null)
            {
                AppendChildScope(s);
                return;
            }
            foreach(var v in child)
            {
                SpreadChildScope(v);
            }
        }

        internal void AppendChildScope(Scope scope)
        {
            if (scope is NameSpace)
            {
                Merge(scope);
            }
            if (string.IsNullOrEmpty(scope.Name))
            {
                return;
            }
            if (!ScopeSymbol.ContainsKey(scope.Name))
            {
                ScopeSymbol.Add(scope.Name, new OverLoad(this));
            }
            ScopeSymbol[scope.Name].Append(scope);//todo 重複判定を実装する。
        }

        protected override string ElementInfo
        {
            get { return Name; }
        }

        internal virtual IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
        {
            yield return TypeMatch.MakeNotCallable(Root.Unknown);
        }

        internal bool IsAnyAttribute(params AttributeType[] type)
        {
            foreach (var v in Attribute)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                if (type.Any(t => t == a.Attr))
                {
                    return true;
                }
            }
            return false;
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
            get { return CurrentScope is ClassSymbol && IsAnyAttribute(AttributeType.Static); }
        }

        public bool IsInstanceMember
        {
            get { return CurrentScope is ClassSymbol && !IsAnyAttribute(AttributeType.Static); }
        }
    }
}
