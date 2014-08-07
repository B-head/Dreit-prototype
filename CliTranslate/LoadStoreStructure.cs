using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LoadStoreStructure : BuilderStructure
    {
        public CilStructure Variant { get; private set; }
        public bool IsStore { get; private set; }

        public LoadStoreStructure(CilStructure variant, bool isStore)
        {
            Variant = variant;
            IsStore = isStore;
        }

        internal override void BuildCall(CodeGenerator cg)
        {
            var f = Variant as FieldStructure;
            if(f != null)
            {
                if(IsStore)
                {
                    cg.GenerateStore(f);
                }
                cg.GenerateLoad(f);
                return;
            }
            var l = Variant as LocalStructure;
            if (l != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(l);
                }
                cg.GenerateLoad(l);
                return;
            }
            var p = Variant as ParameterStructure;
            if(p != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(p);
                }
                cg.GenerateLoad(p);
                return;
            }
            var v = Variant as ValueStructure;
            if(v != null)
            {
                cg.GeneratePrimitive(v.Value);
                return;
            }
            throw new InvalidOperationException();
        }
    }
}
