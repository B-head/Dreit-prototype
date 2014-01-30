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
        private Dictionary<FullName, Translator> TransDictionary { get; set; }

        public AssemblyTranslator(string name)
            : base(name)
        {
            Builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndSave);
            TransDictionary = new Dictionary<FullName, Translator>();
        }

        private string GetSaveName()
        {
            return Name + ".exe";
        }

        public void RegisterEmbed(FullName fullname, Type type)
        {
            EmbedTypeTranslator temp = new EmbedTypeTranslator(fullname, this, type);
            RegisterTranslator(fullname, temp);
        }

        public override Translator FindTranslator(FullName fullName)
        {
            return TransDictionary[fullName];
        }

        public override void RegisterTranslator(FullName fullName, Translator trans)
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

        public override Translator GenelateModule(FullName scope)
        {
            ModuleBuilder builder = Builder.DefineDynamicModule(scope.Name, GetSaveName(), true);
            ModuleTranslator result = new ModuleTranslator(scope, this, builder);
            Child.Add(result);
            return result;
        }
    }
}
