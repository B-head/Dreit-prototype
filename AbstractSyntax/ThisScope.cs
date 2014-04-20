using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax
{
    public class ThisScope : Scope
    {
        private DeclateClass _DataType;

        public ThisScope(DeclateClass dataType)
        {
            Name = "this";
            _DataType = dataType;
        }

        public override DataType DataType
        {
            get { return _DataType; }
        }
    }
}
