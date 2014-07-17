using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class RootStructure : ContainerStructure
    {
        [NonSerialized]
        private AssemblyBuilder Assembly;
        [NonSerialized]
        private ModuleBuilder Module;
        public string Name { get; private set; }
        public string FileName { get; private set; }

        public RootStructure(string name, string dir = null)
        {
            Name = name;
            FileName = name + ".exe";
            Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave, dir);
            Module = Assembly.DefineDynamicModule(Name, FileName);
        }

        internal void TraversalBuildCode()
        {
            TraversalBuildCode(this);
        }

        private void TraversalBuildCode(CilStructure stru)
        {
            RelayBuildCode();
            foreach(var v in stru)
            {
                TraversalBuildCode(v);
            }
        }

        public void Save()
        {
            Assembly.Save(FileName);
        }

        public void Run()
        {
            var entry = Assembly.EntryPoint;
            var type = Assembly.GetType(entry.DeclaringType.FullName);
            entry = type.GetMethod(entry.Name, BindingFlags.NonPublic | BindingFlags.Static);
            entry.Invoke(null, null);
        }

        internal override TypeBuilder CreateType(string name, TypeAttributes attr)
        {
            return Module.DefineType(name, attr);
        }
    }
}
