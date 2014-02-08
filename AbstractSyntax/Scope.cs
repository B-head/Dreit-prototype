using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class Scope : Element, PathNode
    {
        private static int NextId = 1;
        public int Id { get; private set; }
        public string Name { get; set; }
        public FullPath FullPath { get; private set; }
        public Scope ScopeParent { get; private set; }
        private Dictionary<string, Scope> _ScopeChild;
        public IReadOnlyDictionary<string, Scope> ScopeChild { get { return _ScopeChild; } }

        public Scope()
        {
            Id = NextId++;
            _ScopeChild = new Dictionary<string, Scope>();
        }

        private void AddChild(Scope child)
        {
            if(child.Name == null)
            {
                return;
            }
            if (_ScopeChild.ContainsKey(child.Name))
            {
                return;
            }
            _ScopeChild.Add(child.Name, child);
            child.ScopeParent = this;
        }

        private FullPath GetFullPath()
        {
            if (Parent == null)
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
            Scope temp;
            if (_ScopeChild.TryGetValue(name, out temp))
            {
                return temp;
            }
            if (ScopeParent == null)
            {
                return null;
            }
            return ScopeParent.NameResolution(name);
        }

        internal override void SpreadScope(Element parent)
        {
            if (parent != null)
            {
                parent.Scope.AddChild(this);
            }
            base.SpreadScope(parent);
        }

        internal override void CheckSemantic()
        {
            if (ScopeParent == null && !(this is Root))
            {
                CompileError("このスコープには識別子 " + Name + " が既に宣言されています。");
            }
            base.CheckSemantic();
        }

        internal override void SpreadTranslate()
        {
            FullPath = GetFullPath();
            base.SpreadTranslate();
        }
    }
}
