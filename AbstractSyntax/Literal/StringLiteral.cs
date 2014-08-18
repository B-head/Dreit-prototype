using AbstractSyntax.Symbol;
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
        public IReadOnlyList<Element> Texts { get; private set; }
        private TypeSymbol _ReturnType;

        public StringLiteral(TextPosition tp, IReadOnlyList<Element> texts)
            :base(tp)
        {
            Texts = texts;
            AppendChild(Texts);
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                if (_ReturnType == null)
                {
                    _ReturnType = CurrentScope.NameResolution("String").FindDataType().Type;
                }
                return _ReturnType;
            }
        }

        public override bool IsConstant
        {
            get { return true; }
        }
    }
}
