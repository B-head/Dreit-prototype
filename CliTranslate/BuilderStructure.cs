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

        internal void RelayPreBuild()
        {
            if (IsPreBuilded)
            {
                return;
            }
            IsPreBuilded = true;
            BuildCode();
        }

        protected virtual void PreBuild()
        {
            return;
        }

        internal virtual void PostBuild()
        {
            return;
        }

        internal virtual void BuildCall()
        {
            throw new NotSupportedException();
        }
    }
}
