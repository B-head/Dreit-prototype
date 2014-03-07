using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DirectiveList : Element
    {
        public List<Element> Child { get; set; }
        public bool IsInline { get; set; }

        public DirectiveList()
        {
            Child = new List<Element>();
        }

        public void Append(Element append)
        {
            if (append == null)
            {
                return;
            }
            if(Child.Count == 0)
            {
                Position = append.Position;
            }
            Child.Add(append);
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return Child[index];
        }

        internal override void Translate(Translator trans)
        {
            if (IsInline)
            {
                Child[0].Translate(trans);
            }
            else
            {
                foreach (Element v in this)
                {
                    v.Translate(trans);
                    if (!v.IsVoidValue)
                    {
                        trans.GenerateControl(CodeType.Pop);
                    }
                }
            }
            if (Child.Count <= 0 || !(Child.Last() is ReturnDirective))
            {
                trans.GenerateControl(CodeType.Ret);
            }
        }
    }
}
