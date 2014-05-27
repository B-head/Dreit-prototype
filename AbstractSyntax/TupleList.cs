using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class TupleList : Element
    {
        private List<Element> Child;
        private List<IDataType> DataTypes;

        public TupleList()
        {
            Child = new List<Element>();
        }

        public TupleList(Element append)
        {
            Child = new List<Element>();
            Append(append);
        }

        public void Append(Element append)
        {
            Child.Add(append);
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override IElement this[int index]
        {
            get { return Child[index]; }
        }

        public IReadOnlyList<IDataType> GetDataTypes()
        {
            if (DataTypes != null)
            {
                return DataTypes;
            }
            DataTypes = new List<IDataType>();
            foreach(var v in Child)
            {
                DataTypes.Add(v.DataType);
            }
            return DataTypes;
        }
    }
}
