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

        public LocalTranslator(FullName fullname, Translator parent, LocalBuilder builder)
            : base(fullname, parent)
        {
            Builder = builder;
            Builder.SetLocalSymInfo(fullname.Name);
        }

        public override LocalBuilder GetLocal()
        {
            return Builder;
        }

        public override Type GetDataType()
        {
            return Builder.LocalType;
        }

        public override Translator GenelateVariant(FullName gen, FullName type)
        {
            return Parent.GenelateVariant(gen, type);
        }

        public override void GenelateConstant(int value)
        {
            Parent.GenelateConstant(value);
        }

        public override void GenelateConstant(double value)
        {
            Parent.GenelateConstant(value);
        }

        public override void GenelateLoad(FullName type)
        {
            Parent.GenelateLoad(type);
        }

        public override void GenelateStore(FullName type)
        {
            Parent.GenelateStore(type);
        }

        public override void GenelateOperate(FullName type, TokenType operation)
        {
            Parent.GenelateOperate(type, operation);
        }
    }
}
