using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class TemplateInstanceSymbol : Scope
    {
        public OverLoadReference Template { get; private set; }
        public IReadOnlyList<Scope> Parameter { get; private set; }

        public TemplateInstanceSymbol(OverLoadReference template, IReadOnlyList<Scope> parameter)
        {
            Template = template;
            Parameter = parameter;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        public override OverLoadReference OverLoad
        {
            get { return Template; }
        }
    }
}
