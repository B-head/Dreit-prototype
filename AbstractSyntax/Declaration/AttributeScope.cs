using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class AttributeScope : Element
    {
        private List<AttributeSymbol> _Attribute;

        public AttributeScope(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        public IReadOnlyList<AttributeSymbol> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                _Attribute = new List<AttributeSymbol>();
                foreach (var v in this)
                {
                    _Attribute.Add(v.OverLoad.FindAttribute());
                }
                return _Attribute;
            }
        }
    }
}
