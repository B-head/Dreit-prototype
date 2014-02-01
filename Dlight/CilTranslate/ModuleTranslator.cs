using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class ModuleTranslator : ContextTranslator
    {
        public ModuleTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }

        /*private RoutineTranslator CreateGlobalContext(string name)
        {
            MethodAttributes attr = MethodAttributes.Static | MethodAttributes.SpecialName;
            MethodBuilder builder = Builder.DefineGlobalMethod(name, attr, null, null);
            return new RoutineTranslator(name, this, builder);
        }

        public override void BuildCode()
        {
            base.BuildCode();
            ModuleBuilder builder = Builder.DefineDynamicModule(scope.Name, GetSaveName(), true);
            GlobalContext = CreateGlobalContext("@@globalcontext");
            GlobalContext.BuildCode();
            Builder.CreateGlobalFunctions();
        }

        public override Translator CreateAttribute(FullName gen, FullName type)
        {
            return GlobalContext.CreateVariant(gen, type);
        }

        public override Translator CreateRoutine(FullName gen)
        {
            MethodAttributes attr = MethodAttributes.Static;
            MethodBuilder builder = Builder.DefineGlobalMethod(gen.Name, attr, null, null);
            RoutineTranslator result = new RoutineTranslator(gen, this, builder);
            Child.Add(result);
            return result;
        }*/
    }
}
