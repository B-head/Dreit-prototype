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
        public OverLoadReference BaseType { get; private set; }
        public IReadOnlyList<Scope> Parameter { get; private set; }

        public TemplateInstanceSymbol(OverLoadReference baseType, IReadOnlyList<Scope> parameter)
        {
            BaseType = baseType;
            Parameter = parameter;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        public override OverLoadReference OverLoad
        {
            get { return BaseType; }
        }
    }
}
