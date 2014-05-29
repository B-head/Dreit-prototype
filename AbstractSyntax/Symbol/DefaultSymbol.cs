using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class DefaultSymbol : RoutineSymbol
    {
        public DefaultSymbol(ClassSymbol parent)
        {
            _ReturnType = parent;
        }

        protected override void SpreadElement(Element parent, Scope scope)
        {
            base.SpreadElement(parent, scope);
            _ArgumentType = new List<IDataType>();
        }
    }
}
