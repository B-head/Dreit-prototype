using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public abstract class Element : IReadOnlyList<Element>
    {
        public TextPosition Position { get; set; }
        public Element Parent { get; private set; }
        public Root Root { get; private set; }
        public bool IsImport { get; set; }

        internal virtual Scope DataType
        {
            get { return null; }
        }

        internal virtual Scope AccessType
        {
            get { return DataType; }
        }

        public virtual bool IsReference
        {
            get { return false; }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual Element Child(int index)
        {
            throw new ArgumentOutOfRangeException();
        }

        public Element this[int index]
        {
            get { return Child(index); }
        }

        public IEnumerator<Element> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return Child(i);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual string AdditionalInfo()
        {
            return null;
        }

        private string ElementInfo()
        {
            StringBuilder builder = new StringBuilder(Position + " " + this.GetType().Name);
            var add = AdditionalInfo();
            if(add != null)
            {
                builder.Append("(" + add + ")");
            }
            return builder.ToString();
        }

        private string ErrorInfo()
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
            foreach (Element v in this)
            {
                if (v == null)
                {
                    result.AppendLine(Indent(indent + 1) + "<null>");
                    continue;
                }
                if(v.IsImport)
                {
                    continue;
                }
                result.Append(v.ToString(indent + 1));
            }
            return result.ToString();
        }

        protected void SpreadElement(Element parent, Scope scope)
        {
            Parent = parent;
            if (parent == null)
            {
                Root = (Root)this;
            }
            else
            {
                Root = parent.Root;
            }
            if(this is Scope)
            {
                var temp = (Scope)this;
                temp.SpreadScope(scope);
                scope = temp;
            }
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.SpreadElement(this, scope);
                }
            }
        }

        internal virtual void SpreadReference(Scope scope)
        {
            if (this is Scope)
            {
                scope = (Scope)this;
            }
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.SpreadReference(scope);
                }
            }
        }

        internal virtual void CheckSyntax()
        {
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.CheckSyntax();
                }
            }
        }

        internal virtual void CheckDataType(Scope scope)
        {
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.CheckDataType(scope);
                }
            }
        }

        internal virtual void CheckDataTypeAssign(Scope type)
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    continue;
                }
                if (v != null && v.IsReference)
                {
                    v.CheckDataTypeAssign(type);
                }
            }
        }

        internal virtual void Translate(Translator trans)
        {
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.Translate(trans);
                }
            }
        }

        internal virtual void TranslateAssign(Translator trans)
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    continue;
                }
                if (v.IsReference)
                {
                    v.TranslateAssign(trans);
                }
            }
        }
    }
}
