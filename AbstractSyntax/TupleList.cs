using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class TupleList<E> : Element where E : Element
    {
        public List<E> Child { get; set; }

        public TupleList()
        {
            Child = new List<E>();
        }

        public void Append(E append)
        {
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
