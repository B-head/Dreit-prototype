using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.CilTranslate
{
    class OperationTranslator : RoutineTranslator
    {
        public OperationTranslator(TokenType operation, Translator parent)
            : base(null, parent)
        {

        }
    }
}
