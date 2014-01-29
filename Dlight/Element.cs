using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.Translate;

namespace Dlight
{
    abstract class Element
    {
        public TextPosition Position { get; set; }
        public Element Parent { get; set; }
        public Root Root { get; set; }
        public Scope Scope { get; set; }
        public Translator Trans { get; set; }

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
            for(int i = 0; i < ChildCount; i++)
            {
                yield return GetChild(i);
            }
        }

        public virtual string ElementInfo()
        {
            return Position + " " + this.GetType().Name + ": ";
        }

        public virtual string ErrorInfo()
        {
            return Position + ": ";
        }

        public void CompileError(string format, params object[] args)
        {
            Root.OutputError("Error: " + ErrorInfo() + string.Format(format, args));
        }

        public void CompileWarning(string format, params object[] args)
        {
            Root.OutputWarning("Warning: " + ErrorInfo() + string.Format(format, args));
        }

        public virtual void SpreadScope(Scope scope, Element parent)
        {
            Scope = scope;
            Parent = parent;
            if (parent != null)
            {
                Root = parent.Root;
            }
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.SpreadScope(scope, this);
                }
            }
        }

        public virtual void CheckSemantic()
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.CheckSemantic();
                }
            }
        }

        public virtual void CheckDataType()
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.CheckDataType();
                }
            }
        }

        public virtual void CheckDataTypeAssign(string type)
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

        public virtual string GetDataType()
        {
            throw new NotSupportedException();
        }

        public virtual void SpreadTranslate(Translator trans)
        {
            Trans = trans;
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.SpreadTranslate(trans);
                }
            }
        }

        public virtual void Translate()
        {
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.Translate();
                }
            }
        }

        public virtual void TranslateAssign()
        {
            foreach (Element v in EnumChild())
            {
                if(v == null)
                {
                    continue;
                }
                if (v.IsReference)
                {
                    v.TranslateAssign();
                }
            }
        }

        public string Indent(int indent)
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
    }
}
