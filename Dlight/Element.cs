using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight
{
    abstract class Element : IEnumerable<Element>
    {
        public virtual int Count
        {
            get { return 0; }
        }

        public virtual Element this[int index]
        {
            get { throw new ArgumentOutOfRangeException(); }
        }

        public virtual string ElementInfo()
        {
            return this.GetType().Name + ": ";
        }

        public virtual string ErrorInfo()
        {
            return "Error: ";
        }

        public virtual void CreateScope(Scope<Element> scope)
        {
            foreach(Element v in this)
            {
                if(v != null)
                {
                    v.CreateScope(scope);
                }
            }
        }

        public virtual void CheckSemantic(TextWriter output)
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    output.WriteLineAsync(ErrorInfo() + "原因不明の<null>要素が検出されました。");
                    continue;
                }
                v.CheckSemantic(output);
            }
        }

        public virtual void Translate(Translator trans)
        {
            foreach(Element v in this)
            {
                v.Translate(trans);
            }
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public virtual string ToString(int indent)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(Common.Indent(indent) + ElementInfo());
            foreach (Element v in this)
            {
                if (v == null)
                {
                    result.AppendLine("<null>");
                    continue;
                }
                result.Append(v.ToString(indent + 1));
            }
            return result.ToString();
        }

        public IEnumerator<Element> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    abstract class Syntax : Element
    {
        public TextPosition Position { get; set; }

        public override string ElementInfo()
        {
            return Position + " " + base.ElementInfo();
        }

        public override string ErrorInfo()
        {
            return base.ErrorInfo() + Position + ": ";
        }
    }
}
