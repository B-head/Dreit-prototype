using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public abstract class Element
    {
        public TextPosition Position { get; set; }
        public Element Parent { get; private set; }
        public Root Root { get; private set; }
        public Scope Scope { get; private set; }
        public Translator Trans { get; private set; }

        public virtual bool IsReference
        {
            get { return false; }
        }

        public virtual int ChildCount
        {
            get { return 0; }
        }

        public virtual Element GetChild(int index)
        {
            throw new ArgumentOutOfRangeException();
        }

        public IEnumerable<Element> EnumChild()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                yield return GetChild(i);
            }
        }

        protected virtual string ElementInfo()
        {
            return Position + " " + this.GetType().Name + ": ";
        }

        protected virtual string ErrorInfo()
        {
            return Position + ": ";
        }

        protected void CompileError(string message)
        {
            Root.OutputError("Error: " + ErrorInfo() + message);
        }

        protected void CompileWarning(string message)
        {
            Root.OutputWarning("Warning: " + ErrorInfo() + message);
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

        public virtual string ToString(int indent)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(Indent(indent) + ElementInfo());
            foreach (Element v in EnumChild())
            {
                if (v == null)
                {
                    result.AppendLine(Indent(indent + 1) + "<null>");
                    continue;
                }
                result.Append(v.ToString(indent + 1));
            }
            return result.ToString();
        }

        internal virtual void SpreadScope(Element parent)
        {
            Parent = parent;
            if (parent == null)
            {
                Root = (Root)this;
                Scope = (Scope)this;
            }
            else
            {
                Root = parent.Root;
                if (this is Scope)
                {
                    Scope = (Scope)this;
                }
                else
                {
                    Scope = parent.Scope;
                }
            }
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.SpreadScope(this);
                }
            }
        }

        internal virtual void CheckSemantic()
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.CheckSemantic();
                }
            }
        }

        internal virtual void CheckDataType()
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.CheckDataType();
                }
            }
        }

        internal virtual void CheckDataTypeAssign(Scope type)
        {
            foreach (Element v in EnumChild())
            {
                if (v == null)
                {
                    continue;
                }
                if (v.IsReference)
                {
                    v.CheckDataTypeAssign(type);
                }
            }
        }

        internal virtual Scope GetDataType()
        {
            throw new NotSupportedException();
        }

        internal virtual void SpreadTranslate()
        {
            if (Parent != null)
            {
                Trans = CreateTranslator(Parent.Trans);
            }
            else
            {
                Trans = ((Root)this).RootTrans;
            }
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.SpreadTranslate();
                }
            }
        }

        internal virtual Translator CreateTranslator(Translator trans)
        {
            return trans;
        }

        internal virtual void Translate()
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.Translate();
                }
            }
        }

        internal virtual void TranslateAssign()
        {
            foreach (Element v in EnumChild())
            {
                if (v == null)
                {
                    continue;
                }
                if (v.IsReference)
                {
                    v.TranslateAssign();
                }
            }
        }
    }
}
