using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Symbol;
using AbstractSyntax.Daclate;

namespace CliTranslate
{
    public class RootTranslator : Translator
    {
        private Dictionary<IScope, dynamic> BuilderDictonary;
        private AssemblyBuilder Assembly;
        private ModuleBuilder Module;
        public string Name { get; private set; }
        public string FileName { get; private set; }

        public RootTranslator(string name, string dir = null)
            : base(null, null)
        {
            BuilderDictonary = new Dictionary<IScope, dynamic>();
            Name = name;
            FileName = name + ".exe";
            Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave, dir);
            Module = Assembly.DefineDynamicModule(Name, FileName);
        }

        internal bool ContainsBuilder(IScope path)
        {
            if (path is VoidSymbol)
            {
                return true;
            }
            return BuilderDictonary.ContainsKey(path);
        }

        internal dynamic GetBuilder(IScope path)
        {
            if (path is VoidSymbol)
            {
                return typeof(void);
            }
            return BuilderDictonary[path];
        }

        internal Type GetTypeBuilder(IScope path)
        {
            if (path == null)
            {
                return null;
            }
            if (path is VoidSymbol)
            {
                return typeof(void);
            }
            return BuilderDictonary[path];
        }

        internal Type[] GetTypeBuilders(IEnumerable<IScope> path)
        {
            List<Type> result = new List<Type>();
            foreach(var v in path)
            {
                result.Add(GetBuilder(v));
            }
            return result.ToArray();
        }

        internal Type[] GetTypeBuilders(Type prim, IEnumerable<IScope> path)
        {
            List<Type> result = new List<Type>();
            result.Add(prim);
            foreach (var v in path)
            {
                result.Add(GetBuilder(v));
            }
            return result.ToArray();
        }

        public void RegisterBuilder(IScope path, dynamic builder)
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

        public override ModuleTranslator CreateModule(DeclateModule path)
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
