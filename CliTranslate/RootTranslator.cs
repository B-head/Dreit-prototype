using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Symbol;

namespace CliTranslate
{
    public class RootTranslator : Translator
    {
        private Dictionary<Scope, dynamic> BuilderDictonary;
        private Dictionary<Scope, ConstructorInfo> CtorDictonary;
        private AssemblyBuilder Assembly;
        private ModuleBuilder Module;
        public string Name { get; private set; }
        public string FileName { get; private set; }

        public RootTranslator(string name, string dir = null)
            : base(null, null)
        {
            BuilderDictonary = new Dictionary<Scope, dynamic>();
            CtorDictonary = new Dictionary<Scope, ConstructorInfo>();
            Name = name;
            FileName = name + ".exe";
            Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave, dir);
            Module = Assembly.DefineDynamicModule(Name, FileName);
        }

        internal dynamic GetBuilder(Scope path)
        {
            if (path is VoidSymbol)
            {
                return typeof(void);
            }
            return BuilderDictonary[path];
        }

        internal Type GetReturnBuilder(Scope path)
        {
            if (path is VoidSymbol)
            {
                return typeof(void);
            }
            return BuilderDictonary[path];
        }

        internal Type[] GetArgumentBuilders(params Scope[] path)
        {
            List<Type> result = new List<Type>();
            foreach(var v in path)
            {
                result.Add(GetBuilder(v));
            }
            return result.ToArray();
        }

        internal Type[] GetArgumentBuilders(Type prim, params Scope[] path)
        {
            List<Type> result = new List<Type>();
            result.Add(prim);
            foreach (var v in path)
            {
                result.Add(GetBuilder(v));
            }
            return result.ToArray();
        }

        internal ConstructorInfo GetConstructor(Scope path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            return CtorDictonary[path];
        }

        public void RegisterBuilder(Scope path, dynamic builder)
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

        public override ModuleTranslator CreateModule(Scope path)
        {
            return new ModuleTranslator(path, this, Module);
        }

        public override void BuildCode()
        {
            base.BuildCode();
            Module.CreateGlobalFunctions();
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

        internal void SetEntryPoint(MethodBuilder entry)
        {
            Assembly.SetEntryPoint(entry);
        }
    }
}
