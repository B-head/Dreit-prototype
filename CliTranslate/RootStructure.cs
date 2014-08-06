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
        public MethodStructure EntryContext { get; private set; }

        public RootStructure(string name, string dir = null)
        {
            Name = name;
            FileName = name + ".exe";
            Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave, dir);
            Module = Assembly.DefineDynamicModule(Name, FileName);
            var attr = MethodAttributes.PrivateScope | MethodAttributes.SpecialName | MethodAttributes.Static;
            var arg = new List<ParameterStructure>();
            var gnr = new List<GenericParameterStructure>();
            EntryContext = new MethodStructure("@@entry", attr, gnr, arg, null);
            AppendChild(EntryContext);
        }

        internal void TraversalBuildCode()
        {
            TraversalPreBuild(this);
            ChildBuildCode(this);
            TraversalPostBuild(this);
        }

        private void TraversalPreBuild(CilStructure stru)
        {
            var c = stru as BuilderStructure;
            if (c != null)
            {
                c.RelayPreBuild();
            }
            foreach (var v in stru)
            {
                TraversalPreBuild(v);
            }
        }

        private void TraversalPostBuild(CilStructure stru)
        {
            foreach (var v in stru)
            {
                TraversalPostBuild(v);
            }
            var c = stru as BuilderStructure;
            if(c != null)
            {
                c.PostBuild();
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

        internal override void PostBuild()
        {
            Assembly.SetEntryPoint(EntryContext.GainMethod());
            Module.CreateGlobalFunctions();
        }

        internal override CodeGenerator GainGenerator()
        {
            return EntryContext.GainGenerator();
        }

        internal override TypeBuilder CreateType(string name, TypeAttributes attr)
        {
            return Module.DefineType(name, attr);
        }

        internal override MethodBuilder CreateMethod(string name, MethodAttributes attr)
        {
            return Module.DefineGlobalMethod(name, attr, typeof(void), Type.EmptyTypes);
        }
    }
}
