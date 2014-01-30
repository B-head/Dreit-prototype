using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{

    class Scope : Element
    {
        public static int NextId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public FullName FullName { get; set; }
        public Scope ScopeParent { get; set; }
        public Dictionary<string, Scope> ScopeChild { get; set; }

        public Scope()
        {
            Id = NextId++;
            ScopeChild = new Dictionary<string, Scope>();
        }

        private FullName GetFullName()
        {
            if (ScopeParent == null)
            {
                return CreateFullName();
            }
            FullName temp = ScopeParent.GetFullName();
            if(temp == null)
            {
                return CreateFullName();
            }
            temp.Append(Name, Id);
            return temp;
        }

        private FullName CreateFullName()
        {
            if(Name == null)
            {
                return null;
            }
            FullName result = new FullName();
            result.Append(Name, Id);
            return result;
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
                FullName = GetFullName();
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
