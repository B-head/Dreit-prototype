using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class ExpressionList : Element
    {
        public List<Element> Child { get; set; }

        public ExpressionList()
        {
            Child = new List<Element>();
        }

        public void Append(Element append)
        {
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

        public override string ElementInfo()
        {
            return base.ElementInfo() + "Scope = " + Scope.FullName;
        }

        public override void Translate()
        {
            foreach(Element v in EnumChild())
            {
                v.Translate();
                Trans.GenelateStore();
            }
        }
    }
}
