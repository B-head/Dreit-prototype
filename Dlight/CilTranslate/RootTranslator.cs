using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class RootTranslator : PackageTranslator
    {
        public RootTranslator()
            : base(null, null)
        {
            
        }

        public void Save(string saveName)
        {
            base.BuildCode();
            var builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(saveName), AssemblyBuilderAccess.RunAndSave);
            //builder.SetEntryPoint(Child[0].GetContext());
            builder.Save(saveName + ".exe");
        }
    }
}
