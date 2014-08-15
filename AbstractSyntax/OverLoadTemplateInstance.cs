using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class OverLoadTemplateInstance : Scope
    {
        public OverLoad Template { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }

        public OverLoadTemplateInstance(OverLoad template, IReadOnlyList<TypeSymbol> parameter)
        {
            Template = template;
            Parameters = parameter;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        public override OverLoad OverLoad
        {
            get { return Template; }
        }

        public Scope BaseType
        {
            get { return Template.FindDataType(); }
        }
    }
}
