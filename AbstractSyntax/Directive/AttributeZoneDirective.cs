using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class AttributeZoneDirective : Element
    {
        private List<Scope> _Attribute;

        public AttributeZoneDirective(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        public IReadOnlyList<Scope> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                _Attribute = new List<Scope>();
                foreach (var v in this)
                {
                    _Attribute.Add(v.OverLoad.FindDataType());
                }
                return _Attribute;
            }
        }
    }
}
