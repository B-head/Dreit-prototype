using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class LocalTranslator : CilTranslator
    {
        private LocalBuilder Builder { get; set; }

        public LocalTranslator(Scope<Element> scope, CilTranslator parent, LocalBuilder builder)
            : base(scope, parent)
        {
            Builder = builder;
            Builder.SetLocalSymInfo(scope.Name);
        }

        public override LocalBuilder GetLocal()
        {
            return Builder;
        }

        public override Type GetDataType()
        {
            return Builder.LocalType;
        }

        public override Translator CreateVariable(Scope<Element> scope, string fullName)
        {
            return Parent.CreateVariable(scope, fullName);
        }

        public override void GenelateLoad(string fullName)
        {
            Parent.GenelateLoad(fullName);
        }

        public override void GenelateStore(string fullName)
        {
            Parent.GenelateStore(fullName);
        }

        public override void GenelateNumber(int value)
        {
            Parent.GenelateNumber(value);
        }

        public override void GenelateBinomial(string fullName, TokenType operation)
        {
            Parent.GenelateBinomial(fullName, operation);
        }
    }
}
