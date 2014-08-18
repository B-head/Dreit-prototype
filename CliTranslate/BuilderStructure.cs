using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public abstract class BuilderStructure : CilStructure
    {
        private bool IsPreBuilded;
        private bool IsPostBuilded;

        internal void RelayPreBuild()
        {
            if (IsPreBuilded)
            {
                return;
            }
            IsPreBuilded = true;
            PreBuild();
        }

        internal void RelayPostBuild()
        {
            if (IsPostBuilded)
            {
                return;
            }
            IsPostBuilded = true;
            PostBuild();
        }

        protected virtual void PreBuild()
        {
            return;
        }

        internal virtual void PostBuild()
        {
            return;
        }

        internal virtual void BuildCall(CodeGenerator cg)
        {
            throw new NotSupportedException();
        }

        internal virtual BuilderStructure RenewInstance(TypeStructure type)
        {
            throw new NotSupportedException();
        }
    }
}
