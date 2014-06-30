using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AbstractSyntax
{
    public interface IScope : IElement
    {
        string Name { get; }
        IReadOnlyList<IScope> ScopeChild { get; }
        string GetFullName();
    }

    [Serializable]
    public abstract class Scope : Element, IScope
    {
        public string Name { get; set; }
        private Dictionary<string, OverLoad> ScopeSymbol;
        private List<Scope> _ScopeChild;
        public IReadOnlyList<IScope> ScopeChild { get { return _ScopeChild; } }

        public Scope()
        {
            ScopeSymbol = new Dictionary<string, OverLoad>();
            _ScopeChild = new List<Scope>();
        }

        private void Merge(Scope other)
        {
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
            OverLoad temp;
            if(ScopeSymbol.TryGetValue(name, out temp))
            {
                return temp;
            }
            if (this is Root)
            {
                return Root.UnknownOverLoad;
            }
            return CurrentScope.NameResolution(name);
        }

        public string GetFullName()
        {
            StringBuilder builder = new StringBuilder();
            BuildFullName(builder);
            return builder.ToString();
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
                scope.AppendChild(this);
            }
        }

        protected override string GetElementInfo()
        {
            return Name;
        }

        internal virtual IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeNotCallable(Root.Unknown);
        }

        public virtual IDataType ReturnType
        {
            get { return Root.Unknown; }
        }
    }
}
