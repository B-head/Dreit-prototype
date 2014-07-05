using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class TypeManager
    {
        private Root Root;

        public TypeManager(Root root)
        {
            Root = root;
        }
    }
}
