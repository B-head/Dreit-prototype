using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class TupleList : Element
    {
        public List<Element> Child { get; set; }

        public TupleList()
        {
            Child = new List<Element>();
        }

        public void Append(Element append)
        {
            Child.Add(append);
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element this[int index]
        {
            get { return Child[index]; }
        }

        public TupleList GetDataTypes()
        {
            TupleList result = new TupleList();
            foreach(var v in Child)
            {
                result.Append(v.DataType);
            }
            return result;
        }
    }
}
