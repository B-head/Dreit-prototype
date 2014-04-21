using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax.Daclate
{
    public class DeclateOperator: DeclateRoutine
    {
        public TokenType Operator{ get; set; }

        internal override void SpreadReference(Scope scope)
        {
            base.SpreadReference(scope);
            var refer = new List<DataType>();
            foreach (var v in Argument)
            {
                var temp = v.DataType;
                refer.Add(temp);
            }
            ArgumentType = refer;
            if (ExplicitType != null)
            {
                ReturnType = ExplicitType.DataType;
            }
        }
    }
}
