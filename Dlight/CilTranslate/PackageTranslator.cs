using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.CilTranslate
{
    class PackageTranslator : Translator
    {
        public PackageTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }

        public override Translator CreateExturn(Type type)
        {
            return new ExturnTranslator(type, this);
        }

        public override Translator CreatePackage(string name)
        {
            return new PackageTranslator(name, this);
        }

        public override Translator CreateModule(string name)
        {
            return new ModuleTranslator(name, this);
        }
    }
}
