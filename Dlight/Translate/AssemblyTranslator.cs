using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.Translate
{
    class AssemblyTranslator : Translator
    {
        private AssemblyBuilder Builder { get; set; }
        private Dictionary<string, Translator> TransDictionary { get; set; }

        public AssemblyTranslator(string name)
            : base(name)
        {
            Builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndSave);
            TransDictionary = new Dictionary<string, Translator>();
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

        public override Translator FindTranslator(string fullName)
        {
            return TransDictionary[fullName];
        }

        public override void RegisterTranslator(string fullName, Translator trans)
        {
            if(TransDictionary.ContainsKey(fullName))
            {
                return;
            }
            TransDictionary.Add(fullName, trans);
        }

        public override void Save()
        {
            base.Save();
            Builder.SetEntryPoint(Child[0].GetContext());
            Builder.Save(GetSaveName());
        }

        public override Translator GenelateModule(Scope scope)
        {
            ModuleBuilder builder = Builder.DefineDynamicModule(scope.Name, GetSaveName(), true);
            ModuleTranslator result = new ModuleTranslator(scope, this, builder);
            Child.Add(result);
            return result;
        }
    }
}
