using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Common;

namespace CliTranslate
{
    public abstract class Translator
    {
        public RootTranslator Root { get; private set; }
        public Translator Parent { get; private set; }
        private List<Translator> _Child;
        public IReadOnlyList<Translator> Child { get { return _Child; } }
        public FullPath Path { get; private set; }

        protected Translator(FullPath path, Translator parent)
        {
            Path = path;
            _Child = new List<Translator>();
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
            _Child.Add(child);
            child.Parent = this;
        }

        protected string GetSpecialName(string name)
        {
            return "@@" + name + Path.Id;
        }

        protected virtual void Translate()
        {
            foreach (var v in _Child)
            {
                v.Translate();
            }
        }

        private string Indent(int indent)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                result.Append(" ");
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Indent(indent) + this.GetType().Name + ": " + Path.ToString() + "(" + Path.Id + ")");
            foreach (var v in _Child)
            {
                builder.Append(v.ToString(indent + 1));
            }
            return builder.ToString(); 
        }

        public virtual Translator CreateNameSpace(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateClass(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateEnum(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreatePoly(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateGeneric(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateRoutine(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateOperation(VirtualCodeType operation)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateVariant(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateArgument(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual Translator CreateLabel(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual void SetBaseType(FullPath type)
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

        public virtual void GenelateLoad(FullPath type)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateStore(FullPath type)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateCall(FullPath type)
        {
            throw new NotSupportedException();
        }
    }
}
