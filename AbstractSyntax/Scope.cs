using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public abstract class Scope : Element, PathNode
    {
        private static int NextId = 1;
        public int Id { get; private set; }
        public string Name { get; set; }
        public FullPath FullPath { get; private set; }
        public Scope ScopeParent { get; private set; }
        private Dictionary<string, List<Scope>> _ScopeChild;
        public IReadOnlyDictionary<string, List<Scope>> ScopeChild { get { return _ScopeChild; } }
        public bool IsImport { get; set; }

        public Scope()
        {
            Id = NextId++;
            _ScopeChild = new Dictionary<string, List<Scope>>();
        }

        public void AddChild(Scope child)
        {
            child.ScopeParent = this;
            List<Scope> temp;
            if (!_ScopeChild.TryGetValue(child.Name, out temp))
            {
                temp = new List<Scope>();
                _ScopeChild.Add(child.Name, temp);
            }
            temp.Add(child);
        }

        private FullPath GetFullPath()
        {
            if (ScopeParent == null)
            {
                return CreateFullPath();
            }
            var temp = ScopeParent.GetFullPath();
            temp.Append(this);
            return temp;
        }

        private FullPath CreateFullPath()
        {
            var temp = new FullPath();
            temp.Append(this);
            return temp;
        }

        internal Scope NameResolution(string name)
        {
            if (name == Name)
            {
                return this;
            }
            List<Scope> temp;
            if (_ScopeChild.TryGetValue(name, out temp))
            {
                if (temp.Count > 1)
                {
                    return temp[0];
                }
                else
                {
                    return temp[0];
                }
            }
            if (ScopeParent == null)
            {
                return null;
            }
            return ScopeParent.NameResolution(name);
        }

        protected override string AdditionalInfo()
        {
            return Name;
        }

        protected virtual string CreateName()
        {
            return Name;
        }

        internal void SpreadScope(Scope scope)
        {
            Name = CreateName();
            if (scope != null)
            {
                scope.AddChild(this);
            }
            FullPath = GetFullPath();
        }

        protected override void CheckSyntax()
        {
            if (Name == null || Name == string.Empty)
            {
                if (!(this is Root))
                {
                    CompileError(this.GetType().Name + "(ID" + Id + ") の識別子は空です。");
                }
            }
            /*else if (!(this is Root) && false)
            {
                CompileError("識別子 " + Name + " は既に宣言されています。");
            }*/
        }

        internal override void CheckDataType(Scope scope)
        {
            base.CheckDataType(this);
        }
    }
}
