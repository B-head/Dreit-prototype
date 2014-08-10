using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class ElementList<ELEMENT> : Element, IReadOnlyList<ELEMENT> where ELEMENT : Element
    {
        private List<Scope> DataTypes;

        public ElementList()
        {

        }

        public ElementList(TextPosition tp, IReadOnlyList<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        public IReadOnlyList<T> FindElements<T>() where T : ELEMENT
        {
            var result = new List<T>();
            foreach (var v in this)
            {
                if (v is T)
                {
                    result.Add((T)v);
                }
            }
            return result;
        }

        public IReadOnlyList<Scope> GetDataTypes()
        {
            if (DataTypes != null)
            {
                return DataTypes;
            }
            DataTypes = new List<Scope>();
            foreach (var v in this)
            {
                DataTypes.Add(v.ReturnType);
            }
            return DataTypes;
        }

        protected override string ElementInfo
        {
            get { return "Count = " + Count; }
        }

        public new ELEMENT this[int index]
        {
            get { return (ELEMENT)base[index]; }
        }

        public new IEnumerator<ELEMENT> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
