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
        public Scope Template { get; private set; }
        public IReadOnlyList<Scope> Parameter { get; private set; }

        public TemplateInstanceSymbol(Scope template, IReadOnlyList<Scope> parameter)
        {
            Template = template;
            Parameter = parameter;
        }

        public override bool IsDataType
        {
            get { return true; }
        }
    }
}
