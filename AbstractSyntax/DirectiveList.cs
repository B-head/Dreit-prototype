using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

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

        public List<E> FindElements<E>() where E : Element
        {
            var result = new List<E>();
            foreach(var v in Child)
            {
                if(v is E)
                {
                    result.Add((E)v);
                }
            }
            return result;
        }
    }
}
