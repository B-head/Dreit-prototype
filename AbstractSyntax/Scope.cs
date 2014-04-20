using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax
{
    public abstract class Scope : Element, PathNode
    {
        private static int NextId = 1;
        public int Id { get; private set; }
        public string Name { get; set; }
        private ScopeManager ReferenceScope;
        public FullPath FullPath { get; private set; }
        private List<Scope> _ScopeChild;
        public IReadOnlyList<Scope> ScopeChild { get { return _ScopeChild; } }

        public Scope()
        {
            Id = NextId++;
            ReferenceScope = new ScopeManager();
            _ScopeChild = new List<Scope>();
        }

        public void AppendChild(Scope child)
        {
            ReferenceScope.Append(child);
            _ScopeChild.Add(child);
            if(child.IsNameSpace)
            {
                ReferenceScope.Merge(child.ReferenceScope);
            }
        }

        internal Scope NameResolution(string name)
        {
            return NameResolution(name, new List<Scope>());
        }

        internal Scope NameResolution(string name, List<Scope> type)
        {
            Scope temp = ReferenceScope.MatchScope(name, type);
            if(temp != null)
            {
                return temp;
            }
            if (name == Name)
            {
                return this;
            }
            if (ScopeParent == null)
            {
                return null;
            }
            return ScopeParent.NameResolution(name, type);
        }

        public override Scope DataType
        {
            get { return this; }
        }

        public override Scope Reference
        {
            get { return this; }
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

        internal virtual TypeMatchResult TypeMatch(List<Scope> type)
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
