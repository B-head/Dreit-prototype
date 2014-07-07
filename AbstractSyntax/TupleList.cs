using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class TupleList : Element
    {
        private List<Scope> DataTypes;

        public TupleList()
        {

        }

        public TupleList(Element append)
        {
            AppendChild(append);
        }

        public TupleList(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        protected override string ElementInfo
        {
            get { return "Count = " + Count; }
        }

        public IReadOnlyList<Scope> GetDataTypes()
        {
            if (DataTypes != null)
            {
                return DataTypes;
            }
            DataTypes = new List<Scope>();
            foreach(var v in this)
            {
                DataTypes.Add(v.ReturnType);
            }
            return DataTypes;
        }
    }
}
