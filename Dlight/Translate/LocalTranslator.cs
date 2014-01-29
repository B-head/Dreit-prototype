using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.Translate
{
    class LocalTranslator : Translator
    {
        private LocalBuilder Builder { get; set; }

        public LocalTranslator(Scope scope, Translator parent, LocalBuilder builder)
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

        public override Translator GenelateVariant(Scope scope, string fullName)
        {
            return Parent.GenelateVariant(scope, fullName);
        }

        public override void GenelateConstant(int value)
        {
            Parent.GenelateConstant(value);
        }

        public override void GenelateConstant(double value)
        {
            Parent.GenelateConstant(value);
        }

        public override void GenelateLoad(string fullName)
        {
            Parent.GenelateLoad(fullName);
        }

        public override void GenelateStore(string fullName)
        {
            Parent.GenelateStore(fullName);
        }

        public override void GenelateOperate(string fullName, TokenType operation)
        {
            Parent.GenelateOperate(fullName, operation);
        }
    }
}
