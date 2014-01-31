using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.Translate;

namespace Dlight
{
    class SpecialIdentifier : Element
    {
        public string Value { get; set; }

        public override bool IsReference
        {
            get { return true; }
        }

        public override string ElementInfo()
        {
            return base.ElementInfo() + Value;
        }
    }
}
