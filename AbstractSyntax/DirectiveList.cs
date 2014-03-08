using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
