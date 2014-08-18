using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class GenericMethodStructure : BuilderStructure
    {
        public BuilderStructure BaseInstance { get; private set; }
        public IReadOnlyList<TypeStructure> GenericParameter { get; private set; }
        public TypeStructure DeclaringInstance { get; private set; }
        [NonSerialized]
        private MethodInfo Info;

        public void Initialize(BuilderStructure inst, IReadOnlyList<TypeStructure> prms, TypeStructure di, MethodInfo info = null)
        {
            BaseInstance = inst;
            GenericParameter = prms;
            DeclaringInstance = di;
            Info = info;
        }

        public BuilderStructure RenewBaseInstance
        {
            get { return BaseInstance.RenewInstance(DeclaringInstance); }
        }

        internal MethodInfo GainMethod()
        {
            if (Info != null)
            {
                return Info;
            }
            Info = GainBase();
            if(GenericParameter.Count > 0)
            {
                Info = Info.MakeGenericMethod(GenericParameter.GainTypes());
            }
            return Info;
        }

        private MethodInfo GainBase()
        {
            var m = RenewBaseInstance as MethodStructure;
            if (m != null)
            {
                return m.GainMethod();
            }
            throw new InvalidOperationException();
        }

        internal void BuildCall(CilStructure variant, CodeGenerator cg)
        {
            var ls = RenewBaseInstance as LoadStoreStructure;
            if(ls != null)
            {
                ls.BuildCall(variant, cg);
            }
            else if (RenewBaseInstance is MethodStructure)
            {
                cg.GenerateCall(this);
            }
            else
            {
                RenewBaseInstance.BuildCall(cg);
            }
        }
    }
}
