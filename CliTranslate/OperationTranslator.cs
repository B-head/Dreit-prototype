using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    public class OperationTranslator : RoutineTranslator
    {
        public OperationTranslator(VirtualCodeType operation, Translator parent)
            : base(null, parent)
        {

        }
    }
}
