using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;

namespace CliTranslate
{
    public class RootTranslator : Translator
    {
        private Dictionary<FullPath, dynamic> BuilderDictonary;
        private Dictionary<FullPath, ConstructorInfo> CtorDictonary;
        private AssemblyBuilder Assembly;
        private ModuleBuilder Module;
        public string Name { get; private set; }
        public string FileName { get; private set; }

        public RootTranslator(string name)
            : base(null, null)
        {
            BuilderDictonary = new Dictionary<FullPath, dynamic>();
            CtorDictonary = new Dictionary<FullPath, ConstructorInfo>();
            Name = name;
            FileName = name + ".exe";
            Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave);
            Module = Assembly.DefineDynamicModule(Name, FileName);
        }

        internal dynamic GetBuilder(FullPath path)
        {
            if(path == null)
            {
                throw new ArgumentNullException();
            }
            return BuilderDictonary[path];
        }

        internal Type GetReturnBuilder(FullPath path)
        {
            if (path == null)
            {
                return typeof(void);
            }
            return BuilderDictonary[path];
        }

        internal Type[] GetArgumentBuilders(params FullPath[] path)
        {
            List<Type> result = new List<Type>();
            foreach(var v in path)
            {
                result.Add(GetBuilder(v));
            }
            return result.ToArray();
        }

        internal Type[] GetArgumentBuilders(Type prim, params FullPath[] path)
        {
            List<Type> result = new List<Type>();
            result.Add(prim);
            foreach (var v in path)
            {
                result.Add(GetBuilder(v));
            }
            return result.ToArray();
        }

        internal ConstructorInfo GetConstructor(FullPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            return CtorDictonary[path];
        }

        public void RegisterBuilder(FullPath path, dynamic builder)
        {
            if(path == null || builder == null)
            {
                throw new ArgumentNullException();
            }
            if(builder is ConstructorInfo)
            {
                if (!CtorDictonary.ContainsKey(path))
                {
                    CtorDictonary.Add(path, builder);
                }
            }
            else
            {
                if (!BuilderDictonary.ContainsKey(path))
                {
                    BuilderDictonary.Add(path, builder);
                }
            }
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
