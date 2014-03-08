using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal override Scope DataType
        {
            get { return _DataType; }
        }
    }
}
