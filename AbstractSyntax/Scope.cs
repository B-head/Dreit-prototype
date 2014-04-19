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
        public FullPath FullPath { get; private set; }
        private Dictionary<string, Scope> _ScopeChild;
        public IReadOnlyDictionary<string, Scope> ScopeChild { get { return _ScopeChild; } }

        public Scope()
        {
            Id = NextId++;
            _ScopeChild = new Dictionary<string, Scope>();
        }

        public void AppendChild(Scope child)
        {
            Scope temp;
            if (!_ScopeChild.TryGetValue(child.Name, out temp))
            {
                _ScopeChild.Add(child.Name, child);
            }
        }

        private FullPath GetFullPath()
        {
            if (ScopeParent == null)
            {
                return new FullPath();
            }
            var temp = ScopeParent.GetFullPath();
            temp.Append(this);
            return temp;
        }

        internal Scope NameResolution(string name)
        {
            Scope temp = ChildNameResolution(name);
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
            return ScopeParent.NameResolution(name);
        }

        private Scope ChildNameResolution(string name)
        {
            Scope temp;
            if (_ScopeChild.TryGetValue(name, out temp))
            {
                return temp;
            }
            var deccls = this as DeclateClass;
            if(deccls != null)
            {
                foreach(var v in deccls.InheritRefer)
                {
                    temp = v.ChildNameResolution(name);
                    if (temp != null)
                    {
                        return temp;
                    }
                }
            }
            foreach(var peir in _ScopeChild)
            {
                var v = peir.Value;
                if (v.IsNameSpace)
                {
                    temp = v.ChildNameResolution(name);
                    if(temp != null)
                    {
                        return temp;
                    }
                }
            }
            return null;
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

        internal void SpreadScope(Scope scope)
        {
            Name = CreateName();
            if (scope != null)
            {
                scope.AppendChild(this);
            }
            FullPath = GetFullPath();
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
