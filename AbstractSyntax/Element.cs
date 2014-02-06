using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CilTranslate;
using Common;

namespace AbstractSyntax
{
    public abstract class Element
    {
        public TextPosition Position { get; set; }
        public Element Parent { get; private set; }
        public Root Root { get; private set; }
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
            for(int i = 0; i < ChildCount; i++)
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

        protected Element NameResolution(string name)
        {
            return Root.GetPeir(Trans.NameResolution(name));
        }

        protected void SpreadScope(Translator trans, Element parent)
        {
            Parent = parent;
            Trans = CreateTranslator(trans);
            if (parent == null)
            {
                Root = (Root)this;
            }
            else
            {
                Root = parent.Root;
            }
            if(trans != Trans)
            {
                Root.RegisterPeir(this);
            }
            foreach (Element v in EnumChild())
            {
                if (v != null)
                {
                    v.SpreadScope(Trans, this);
                }
            }
        }

        protected virtual Translator CreateTranslator(Translator trans)
        {
            return trans;
        }

        public virtual void CheckSemantic()
        {
            if (Trans.Parent == null && !(Trans is RootTranslator))
            {
                CompileError("このスコープには識別子 " + Trans.Name + " が既に宣言されています。");
            }
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

        public virtual void CheckDataTypeAssign(Translator type)
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

        public virtual Translator GetDataType()
        {
            throw new NotSupportedException();
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
