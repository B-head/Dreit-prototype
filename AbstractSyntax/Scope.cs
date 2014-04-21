using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax
{
    public abstract class Scope : Element
    {
        private static int NextId = 1;
        public int Id { get; private set; }
        public string Name { get; set; }
        private Dictionary<string, OverLoadScope> ScopeSymbol;
        private List<Scope> _ScopeChild;
        public IReadOnlyList<Scope> ScopeChild { get { return _ScopeChild; } }

        public Scope()
        {
            Id = NextId++;
            ScopeSymbol = new Dictionary<string, OverLoadScope>();
            _ScopeChild = new List<Scope>();
        }

        public void AppendChild(Scope scope)
        {
            OverLoadScope ol;
            if (ScopeSymbol.ContainsKey(scope.Name))
            {
                ol = ScopeSymbol[scope.Name];
            }
            else
            {
                ol = new OverLoadScope();
                ScopeSymbol[scope.Name] = ol;
            }
            ol.Append(scope);//todo 重複判定を実装する。
            _ScopeChild.Add(scope);
            if (scope.IsNameSpace)
            {
                Merge(scope);
            }
        }

        private void Merge(Scope other)
        {
            foreach (var v in other.ScopeSymbol)
            {
                OverLoadScope ol;
                if (ScopeSymbol.ContainsKey(v.Key))
                {
                    ol = ScopeSymbol[v.Key];
                }
                else
                {
                    ol = new OverLoadScope();
                    ScopeSymbol[v.Key] = ol;
                }
                ol.Merge(v.Value);
            }
        }

        internal OverLoadScope NameResolution(string name)
        {
            OverLoadScope temp;
            if(ScopeSymbol.TryGetValue(name, out temp))
            {
                return temp;
            }
            if (name == Name)
            {
                //return this;
            }
            if (ScopeParent == null)
            {
                return null;
            }
            return ScopeParent.NameResolution(name);
        }

        public string GetFullName()
        {
            StringBuilder builder = new StringBuilder();
            BuildFullName(builder);
            return builder.ToString();
        }

        private void BuildFullName(StringBuilder builder)
        {
            if(ScopeParent != null && !(ScopeParent is Root))
            {
                ScopeParent.BuildFullName(builder);
                builder.Append(".");
            }
            builder.Append(Name);
        }

        internal virtual bool IsNameSpace
        {
            get { return false; }
        }

        protected override string AdditionalInfo()
        {
            return Name;
        }

        protected virtual string CreateName()
        {
            return Name;
        }

        internal virtual TypeMatchResult TypeMatch(List<DataType> type)
        {
            if(type.Count == 0)
            {
                return TypeMatchResult.PerfectMatch;
            }
            else
            {
                return TypeMatchResult.NotCallable;
            }
        }

        internal void SpreadScope(Scope scope)
        {
            Name = CreateName();
            if (scope != null)
            {
                scope.AppendChild(this);
            }
        }

        internal override void CheckSyntax()
        {
            if (Name == null || Name == string.Empty)
            {
                if (!(this is Root))
                {
                    CompileError(this.GetType().Name + "(ID" + Id + ") の識別子は空です。");
                }
            }
            else if (!(this is Root) && false)
            {
                CompileError("識別子 " + Name + " は既に宣言されています。");
            }
            base.CheckSyntax();
        }
    }
}
