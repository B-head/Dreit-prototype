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

        public override void GenelateLoad(FullName fullName)
        {
            Parent.GenelateLoad(fullName);
        }

        public override void GenelateStore(FullName fullName = null)
        {
            Parent.GenelateStore(fullName);
        }
    }
}
