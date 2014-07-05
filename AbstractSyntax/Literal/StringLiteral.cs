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
        private Lazy<IDataType> LazyReturnType;

        public StringLiteral(TextPosition tp, List<Element> texts)
            :base(tp)
        {
            _Texts = texts;
            LazyReturnType = new Lazy<IDataType>(InitReturnType);
        }

        public override int Count
        {
            get { return _Texts.Count; }
        }

        public override IElement this[int index]
        {
            get { return _Texts[index]; }
        }

        public IReadOnlyList<Element> Texts
        {
            get { return _Texts; }
        }

        public override IDataType ReturnType
        {
            get { return LazyReturnType.Value; }
        }

        private IDataType InitReturnType()
        {
            return CurrentScope.NameResolution("String").FindDataType();
        }

        public void Append(Element value)
        {
            _Texts.Add(value);
        }
    }
}
