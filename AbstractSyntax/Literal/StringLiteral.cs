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
        private Scope _ReturnType;

        public StringLiteral(TextPosition tp, List<Element> texts)
            :base(tp)
        {
            AppendChild(texts);
        }

        public IReadOnlyList<Element> Texts
        {
            get { return this; }
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

        public override bool IsConstant
        {
            get { return true; }
        }
    }
}
