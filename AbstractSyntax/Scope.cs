using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AbstractSyntax
{
    public interface IScope : IElement
    {
        string Name { get; }
        IReadOnlyList<IScope> ScopeChild { get; }
        string FullName { get; }
    }

    [Serializable]
    public abstract class Scope : Element, IScope
    {
        public string Name { get; internal set; }
        private Dictionary<string, OverLoad> ScopeSymbol;
        private List<Scope> _ScopeChild;
        public IReadOnlyList<IScope> ScopeChild { get { return _ScopeChild; } }

        protected Scope()
        {
            ScopeSymbol = new Dictionary<string, OverLoad>();
            _ScopeChild = new List<Scope>();
        }

        protected Scope(TextPosition tp)
            : base(tp)
        {
            ScopeSymbol = new Dictionary<string, OverLoad>();
            _ScopeChild = new List<Scope>();
        }

        protected void Merge(Scope other)
        {
            if(other == null)
            {
                throw new ArgumentNullException("other");
            }
            foreach (var v in other.ScopeSymbol)
            {
                OverLoad ol;
                if (ScopeSymbol.ContainsKey(v.Key))
                {
                    ol = ScopeSymbol[v.Key];
                }
                else
                {
                    ol = new OverLoad(Root.Unknown);
                    ScopeSymbol[v.Key] = ol;
                }
                ol.Merge(v.Value);
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
            OverLoad temp;
            if (ScopeSymbol.TryGetValue(name, out temp))
            {
                return temp;
            }
            return Root.UndefinedOverLord;
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

        private void AppendChild(Scope scope)
        {
            _ScopeChild.Add(scope);
            if (string.IsNullOrEmpty(scope.Name))
            {
                return;
            }
            OverLoad ol;
            if (ScopeSymbol.ContainsKey(scope.Name))
            {
                ol = ScopeSymbol[scope.Name];
            }
            else
            {
                ol = new OverLoad(Root.Unknown);
                ScopeSymbol[scope.Name] = ol;
            }
            ol.Append(scope);//todo 重複判定を実装する。
            if (scope is NameSpace)
            {
                Merge(scope);
            }
        }

        protected override void SpreadElement(Element parent, Scope scope)
        {
            base.SpreadElement(parent, scope);
            if (!(this is Root))
            {
                if(scope == null)
                {
                    throw new ArgumentNullException("scope");
                }
                scope.AppendChild(this);
            }
        }

        protected override string ElementInfo
        {
            get { return Name; }
        }

        internal virtual IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
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

        public virtual IDataType CallReturnType
        {
            get { return Root.Unknown; }
        }

        public virtual IReadOnlyList<IScope> Attribute
        {
            get { return new List<IScope>(); }
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
