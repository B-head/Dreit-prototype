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
        private List<IScope> _Attribute;

        public AttributeZoneDirective()
        {
            Child = new List<Element>();
        }

        public AttributeZoneDirective(List<Element> child)
        {
            Child = child;
        }

        public void Append(Element value)
        {
            Child.Add(value);
        }

        public IReadOnlyList<IScope> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                _Attribute = new List<IScope>();
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

        public override IElement this[int index]
        {
            get { return Child[index]; }
        }
    }
}
