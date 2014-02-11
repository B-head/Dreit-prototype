using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class ExpressionList : Element
    {
        public List<Element> Child { get; set; }

        public ExpressionList()
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

        public override int ChildCount
        {
            get { return Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return Child[index];
        }

        internal override void Translate(Translator trans)
        {
            foreach(Element v in EnumChild())
            {
                v.Translate(trans);
                trans.GenelateControl(CodeType.Pop);
            }
            trans.GenelateControl(CodeType.Ret);
        }
    }
}
