using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class EnumStructure : TypeStructure
    {
        public void Initialize()
        {
            base.Initialize(null, TypeAttributes.Class);
        }
    }
}
