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
        public void Initialize(string name, TypeAttributes attr, BlockStructure block = null, Type info = null)
        {
            base.Initialize(name, attr, block, info);
        }
    }
}
