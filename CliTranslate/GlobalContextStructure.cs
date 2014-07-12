using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class GlobalContextStructure : ContainerStructure
    {
        public string Name { get; private set; }
        public TypeStructure GlobalField { get; private set; }
        public MethodStructure EntryContext { get; private set; }

        public GlobalContextStructure(string name)
        {
            Name = name;
        }
    }
}
