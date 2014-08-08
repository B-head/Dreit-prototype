using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Expression
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

        public override bool IsConstant
        {
            get 
            {
                foreach(var v in this)
                {
                    if(!v.IsConstant)
                    {
                        return false;
                    }
                }
                return true;
            }
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
