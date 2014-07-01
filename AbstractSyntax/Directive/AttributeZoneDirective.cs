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

        public void Append(Element append)
        {
            Child.Add(append);
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
                    _Attribute.Add(v.Reference.GetDataType());
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
