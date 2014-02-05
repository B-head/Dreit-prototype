using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class RootTranslator : NameSpaceTranslator
    {
        public AssemblyBuilder Assembly { get; private set; }
        public ModuleBuilder Module { get; private set; }

        public RootTranslator()
            : base(null, null)
        {
            
        }

        public void RegisterExtern(Assembly assembly)
        {
            var module = assembly.GetModules();
            foreach(var v in module)
            {
                var field = v.GetFields();
                foreach(var f in field)
                {
                    if(!f.IsPublic)
                    {
                        continue;
                    }
                    var name = f.Name.Split('.').ToList();
                    RegisterField(name, f);
                }
                var method = v.GetMethods();
                foreach(var m in method)
                {
                    if (!m.IsPublic)
                    {
                        continue;
                    }
                    var name = m.Name.Split('.').ToList();
                    RegisterMethod(name, m);
                }
                var type = v.GetTypes();
                foreach(var t in type)
                {
                    if(!t.IsPublic)
                    {
                        continue;
                    }
                    var name = t.FullName.Split('.').ToList();
                    RegisterType(name, t);
                }
            }
        }

        public void Save(string saveName)
        {
            var fileName = saveName + ".exe";
            Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(saveName), AssemblyBuilderAccess.RunAndSave);
            Module = Assembly.DefineDynamicModule(saveName, fileName);
            base.SpreadBuilder();
            base.Translate();
            Module.CreateGlobalFunctions();
            Assembly.Save(fileName);
        }
    }
}
