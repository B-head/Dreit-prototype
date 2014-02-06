using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CilTranslate
{
    public class OperationTranslator : RoutineTranslator
    {
        public OperationTranslator(VirtualCodeType operation, Translator parent)
            : base(null, parent)
        {

        }
    }
}
