using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LoadStoreStructure : BuilderStructure
    {
        public bool IsStore { get; private set; }

        public LoadStoreStructure(bool isStore)
        {
            IsStore = isStore;
        }

        internal void BuildCall(CilStructure veriant, CodeGenerator cg)
        {
            var f = veriant as FieldStructure;
            if(f != null)
            {
                if (IsStore)
                {
                    if(f.IsStatic)
                    {
                        cg.GenerateStore(f);
                        cg.GenerateLoad(f);
                    }
                    else
                    {
                        cg.GenerateControl(OpCodes.Dup);
                        var temp = new LocalStructure(f.DataType, cg);
                        cg.GenerateStore(temp);
                        cg.GenerateStore(f);
                        cg.GenerateLoad(temp);
                    }
                }
                else
                {
                    cg.GenerateLoad(f);
                }
                return;
            }
            var l = veriant as LocalStructure;
            if (l != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(l);
                }
                cg.GenerateLoad(l);
                return;
            }
            var p = veriant as ParameterStructure;
            if(p != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(p);
                }
                cg.GenerateLoad(p);
                return;
            }
            var lo = veriant as LoopParameterStructure;
            if (lo != null)
            {
                if (IsStore)
                {
                    cg.GenerateStore(lo.Local);
                }
                cg.GenerateLoad(lo.Local);
                return;
            }
            var v = veriant as ValueStructure;
            if(v != null)
            {
                cg.GeneratePrimitive(v.Value);
                return;
            }
            throw new InvalidOperationException();
        }
    }
}
