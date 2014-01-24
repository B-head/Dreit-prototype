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

        public ModuleTranslator(Scope<Element> scope, AssemblyTranslator parent, ModuleBuilder builder)
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

        public override Type GetDataType()
        {
            throw new NotSupportedException(); //どうするか考えよう。
        }

        public override void Save()
        {
            base.Save();
            GlobalContext.Save();
            Builder.CreateGlobalFunctions();
        }

        public override void GenelateNumber(int value)
        {
            GlobalContext.GenelateNumber(value);
        }

        public override void GenelateBinomial(string fullName, SyntaxType operation)
        {
            GlobalContext.GenelateBinomial(fullName, operation);
        }
    }
}
