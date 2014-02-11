using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

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

        internal override void Translate()
        {
            foreach(Element v in EnumChild())
            {
                v.Translate();
                //GetTranslator().GenelateControl(VirtualCodeType.Pop);
            }
        }
    }
}
