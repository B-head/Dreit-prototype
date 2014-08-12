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
        public ElementList()
        {

        }

        public ElementList(TextPosition tp, IReadOnlyList<Element> child)
            :base(tp)
        {
            AppendChild(child);
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
