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
        private List<Element> Child;
        private List<Scope> _Attribute;

        public AttributeZoneDirective(TextPosition tp, List<Element> child)
            :base(tp)
        {
            Child = child;
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
                foreach (var v in Child)
                {
                    _Attribute.Add(v.Reference.FindDataType());
                }
                return _Attribute;
            }
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element this[int index]
        {
            get { return Child[index]; }
        }
    }
}
