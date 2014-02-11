using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Common;

namespace CliTranslate
{
    public class RootTranslator : Translator
    {
        private Dictionary<FullPath, dynamic> BuilderDictonary;
        private List<Assembly> ImportAssembly;
        private AssemblyBuilder Assembly;
        private ModuleBuilder Module;
        public string Name { get; private set; }
        public string FileName { get; private set; }

        public RootTranslator(string name)
            : base(null, null)
        {
            BuilderDictonary = new Dictionary<FullPath, object>();
            ImportAssembly = new List<Assembly>();
            Name = name;
            FileName = name + ".exe";
            Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave);
            Module = Assembly.DefineDynamicModule(Name, FileName);
        }

        public void AppendAssembly(Assembly assembly)
        {
            ImportAssembly.Add(assembly);
        }

        internal dynamic GetBuilder(FullPath path)
        {
            if(path == null)
            {
                throw new ArgumentNullException();
            }
            return BuilderDictonary[path];
        }

        internal void RegisterBuilder(FullPath path, dynamic builder)
        {
            if(path == null || builder == null)
            {
                throw new ArgumentNullException();
            }
            if (!BuilderDictonary.ContainsKey(path))
            {
                BuilderDictonary.Add(path, builder);
            }
        }

        internal Type GetImportType(string name)
        {
            foreach(var v in ImportAssembly)
            {
                var t = v.GetType(name);
                if(t != null)
                {
                    return t;
                }
            }
            return null;
        }

        public override ModuleTranslator CreateModule(FullPath path)
        {
            return new ModuleTranslator(path, this, Module);
        }

        public override void Save()
        {
            base.Save();
            Module.CreateGlobalFunctions();
            Assembly.Save(FileName);
        }

        internal void SetEntryPoint(MethodBuilder entry)
        {
            Assembly.SetEntryPoint(entry);
        }
    }
}
