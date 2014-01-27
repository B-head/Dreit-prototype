using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{

    class Scope : Element
    {
        public string Name { get; set; }
        public Scope ScopeParent { get; set; }
        public Dictionary<string, Scope> ScopeChild { get; set; }

        public Scope()
        {
            ScopeChild = new Dictionary<string, Scope>();
        }

        public string GetFullName()
        {
            if (ScopeParent == null)
            {
                return Name;
            }
            string p = ScopeParent.GetFullName();
            return p == null ? Name : p + "." + Name;
        }

        public Scope NameResolution(string name)
        {
            if (name == Name)
            {
                return this;
            }
            Scope temp;
            if (ScopeChild.TryGetValue(name, out temp))
            {
                return temp;
            }
            if (ScopeParent == null)
            {
                return null;
            }
            return ScopeParent.NameResolution(name);
        }

        public void AddChild(Scope add)
        {
            if(ScopeChild.ContainsKey(add.Name))
            {
                return;
            }
            ScopeChild.Add(add.Name, add);
            add.ScopeParent = this;
        }

        public override void SpreadScope(Scope scope, Element parent)
        {
            if (scope != null)
            {
                scope.AddChild(this);
            }
            base.SpreadScope(this, parent);
        }

        public override void CheckSemantic()
        {
            if(ScopeParent == null && this != Root)
            {
                CompileError("このスコープには識別子 " + Name + " が既に宣言されています。");
            }
            base.CheckSemantic();
        }
    }
}
