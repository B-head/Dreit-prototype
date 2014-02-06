using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace CilTranslate
{
    public abstract class Translator
    {
        private static int NextId;
        public int Id { get; private set; }
        public string Name { get; private set; }
        public RootTranslator Root { get; private set; }
        public Translator Parent { get; private set; }
        private Dictionary<string, Translator> _Child;
        public IReadOnlyDictionary<string, Translator> Child { get { return _Child; } }

        protected Translator(string name, Translator parent)
        {
            Id = NextId++;
            Name = name;
            _Child = new Dictionary<string, Translator>();
            if(parent == null)
            {
                Root = (RootTranslator)this;
            }
            else
            {
                Root = parent.Root;
                parent.AddChild(this);
            }
        }

        private void AddChild(Translator child)
        {
            if (_Child.ContainsKey(child.Name))
            {
                return;
            }
            _Child.Add(child.Name, child);
            child.Parent = this;
        }

        protected string GetFullName()
        {
            if(Parent == null)
            {
                return Name;
            }
            string temp = Parent.GetFullName();
            return temp == null ? Name : temp + "." + Name;
        }

        protected string GetSpecialName(string name)
        {
            return "@@" + name + Id;
        }

        public Translator NameResolution(string name)
        {
            if (name == Name)
            {
                return this;
            }
            Translator temp;
            if (_Child.TryGetValue(name, out temp))
            {
                return temp;
            }
            if (Parent == null)
            {
                return null;
            }
            return Parent.NameResolution(name);
        }

        protected virtual void SpreadBuilder()
        {
            foreach (var v in _Child)
            {
                v.Value.SpreadBuilder();
            }
        }

        protected virtual void Translate()
        {
            foreach (var v in _Child)
            {
                v.Value.Translate();
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + ": " + GetFullName() + "(" + Id + ")"; 
        }

        public virtual Translator CreateNameSpace(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateClass(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateEnum(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreatePoly(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateGeneric(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateRoutine(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateOperation(VirtualCodeType operation)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateVariant(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateArgument(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateLabel(string name)
        {
            throw new NotSupportedException();
        }

        public virtual void SetBaseType(Translator type)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateControl(VirtualCodeType type)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelatePrimitive(object value)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateLoad(Translator type)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateStore(Translator type)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateCall(Translator type)
        {
            throw new NotSupportedException();
        }
    }
}
