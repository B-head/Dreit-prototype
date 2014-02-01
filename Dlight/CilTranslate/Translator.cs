using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    abstract class Translator
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

        protected virtual void BuildCode()
        {
            foreach (var v in _Child)
            {
                v.Value.BuildCode();
            }
        }

        public virtual Translator CreateExturn(Type type)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreatePackage(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateModule(string name)
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

        public virtual Translator CreateOperation(TokenType operation)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateVariant(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateAttribute(string name)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateLabel(string name)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelatePrimitive(object value)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateControl(VirtualCodeType type)
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
