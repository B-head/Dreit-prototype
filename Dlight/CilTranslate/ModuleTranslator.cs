using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class ModuleTranslator : CilTranslator
    {
        private ModuleBuilder Builder { get; set; }
        private RoutineTranslator GlobalContext { get; set; }

        public ModuleTranslator(Scope<Element> scope, CilTranslator parent, ModuleBuilder builder)
            : base(scope, parent)
        {
            Builder = builder;
            GlobalContext = CreateGlobalContext("@@globalcontext");
        }

        private RoutineTranslator CreateGlobalContext(string name)
        {
            MethodAttributes attr = MethodAttributes.Static | MethodAttributes.SpecialName;
            MethodBuilder builder = Builder.DefineGlobalMethod(name, attr, typeof(void), Type.EmptyTypes);
            return new RoutineTranslator(name, this, builder);
        }

        public override MethodInfo GetContext()
        {
            return GlobalContext.GetContext();
        }

        public override void Save()
        {
            base.Save();
            GlobalContext.Save();
            Builder.CreateGlobalFunctions();
        }

        public override Translator CreateVariable(Scope<Element> scope, string fullName)
        {
            return GlobalContext.CreateVariable(scope, fullName);
        }

        public override void GenelateLoad(string fullName)
        {
            GlobalContext.GenelateLoad(fullName);
        }

        public override void GenelateStore(string fullName)
        {
            GlobalContext.GenelateStore(fullName);
        }

        public override void GenelateNumber(int value)
        {
            GlobalContext.GenelateNumber(value);
        }

        public override void GenelateBinomial(string fullName, TokenType operation)
        {
            GlobalContext.GenelateBinomial(fullName, operation);
        }
    }
}
