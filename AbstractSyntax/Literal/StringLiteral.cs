using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class StringLiteral : Element
    {
        private List<Element> _Texts;
        private Scope _ReturnType;

        public StringLiteral(TextPosition tp, List<Element> texts)
            :base(tp)
        {
            _Texts = texts;
        }

        public override int Count
        {
            get { return _Texts.Count; }
        }

        public override Element this[int index]
        {
            get { return _Texts[index]; }
        }

        public IReadOnlyList<Element> Texts
        {
            get { return _Texts; }
        }

        public override Scope ReturnType
        {
            get
            {
                if (_ReturnType == null)
                {
                    _ReturnType = CurrentScope.NameResolution("String").FindDataType();
                }
                return _ReturnType;
            }
        }

        public void Append(Element value)
        {
            _Texts.Add(value);
        }
    }
}
