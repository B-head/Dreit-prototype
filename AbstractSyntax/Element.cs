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
            get { throw new NotSupportedException(); }
        }

        public virtual bool IsAssignable
        {
            get { return false; }
        }

        public virtual bool IsVoidValue
        {
            get { return false; }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual Element GetChild(int index)
        {
            throw new ArgumentOutOfRangeException();
        }

        public Element this[int index]
        {
            get { return GetChild(index); }
        }

        public IEnumerator<Element> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetChild(i);
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
                builder.Append(": " + add);
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

        protected void CompileError(string message, TextPosition position)
        {
            Root.OutputError("Error: " + position + ": " + message);
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
            return ToString(false);
        }

        public virtual string ToString(bool hideImport, int indent = 0)
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
                if(hideImport && v.IsImport)
                {
                    continue;
                }
                result.Append(v.ToString(hideImport, indent + 1));
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

        internal virtual void CheckDataType()
        {
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.CheckDataType();
                }
            }
        }

        internal virtual void Translate(Translator trans)
        {
            foreach (Element v in this)
            {
                if (v != null && !v.IsImport)
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
                if (v.IsAssignable)
                {
                    v.TranslateAssign(trans);
                }
            }
        }
    }
}
