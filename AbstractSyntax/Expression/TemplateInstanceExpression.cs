using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class TemplateInstanceExpression : Element
    {
        public Element Access { get; private set; }
        public TupleList DecParameters { get; private set; }
        private IReadOnlyList<Scope> _Parameter;

        public TemplateInstanceExpression(TextPosition tp, Element acs, TupleList args)
            : base(tp)
        {
            Access = acs;
            DecParameters = args;
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Access;
                    case 1: return DecParameters;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public override Scope ReturnType
        {
            get { return Root.TypeManager.IssueTemplateInstance(Access.ReturnType, Parameter.ToArray()); }
        }

        public IReadOnlyList<Scope> Parameter
        {
            get
            {
                if (_Parameter != null)
                {
                    return _Parameter;
                }
                var pt = new List<Scope>();
                foreach (var v in DecParameters)
                {
                    var temp = v.Reference.FindDataType();
                    pt.Add(temp);
                }
                _Parameter = pt;
                return _Parameter;
            }
        }

    }
}
