using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class AssemblyTranslator : CilTranslator
    {
        private AssemblyBuilder Builder { get; set; }
        private Dictionary<string, CilTranslator> TransDictionary { get; set; }

        public AssemblyTranslator(string name)
            : base(name)
        {
            Builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndSave);
            TransDictionary = new Dictionary<string, CilTranslator>();
        }

        private string GetSaveName()
        {
            return Name + ".exe";
        }

        public void RegisterEmbedType(string name, Type type)
        {
            EmbedTypeTranslator temp = new EmbedTypeTranslator(name, type);
            RegisterTranslator(name, temp);
        }

        public override CilTranslator FindTranslator(string fullName)
        {
            return TransDictionary[fullName];
        }

        public override void RegisterTranslator(string fullName, CilTranslator trans)
        {
            TransDictionary.Add(fullName, trans);
        }

        public override void Save()
        {
            base.Save();
            Builder.SetEntryPoint(Child[0].GetContext());
            Builder.Save(GetSaveName());
        }

        public override Translator CreateModule(Scope<Element> scope)
        {
            ModuleBuilder builder = Builder.DefineDynamicModule(scope.Name, GetSaveName());
            ModuleTranslator result = new ModuleTranslator(scope, this, builder);
            Child.Add(result);
            return result;
        }
    }
}
