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
        public Scope DataType { get; protected set; }

        internal virtual Scope AccessType
        {
            get { return DataType; }
        }

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
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.SpreadElement(this, scope);
                }
            }
            CheckSyntax();
        }

        protected virtual void CheckSyntax()
        {
            return;
        }

        internal virtual void CheckDataType(Scope scope)
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.CheckDataType(scope);
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
                if (v != null && v.IsReference)
                {
                    v.CheckDataTypeAssign(type);
                }
            }
        }

        internal virtual void Translate(Translator trans)
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.Translate(trans);
                }
            }
        }

        internal virtual void TranslateAssign(Translator trans)
        {
            foreach (Element v in EnumChild())
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
