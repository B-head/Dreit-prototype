using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class TupleList : Element
    {
        public List<Element> _Child { get; set; }

        public TupleList()
        {
            _Child = new List<Element>();
        }

        public void Append(Element append)
        {
            _Child.Add(append);
        }

        public override int Count
        {
            get { return _Child.Count; }
        }

        public override Element Child(int index)
        {
            return _Child[index];
        }
    }
}
