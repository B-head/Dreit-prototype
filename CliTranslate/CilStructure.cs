using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public abstract class CilStructure : IReadOnlyList<CilStructure>
    {
        private List<CilStructure> Child;
        public CilStructure Parent { get; private set; }

        protected CilStructure()
        {
            Child = new List<CilStructure>();
        }

        internal void AppendChild(CilStructure child)
        {
            if (child == null)
            {
                return;
            }
            Child.Add(child);
            child.RegisterParent(this);
        }

        private void RegisterParent(CilStructure parent)
        {
            Parent = parent;
        }

        public int Count
        {
            get { return Child.Count; }
        }

        public CilStructure this[int index]
        {
            get { return Child[index]; }
        }

        public IEnumerator<CilStructure> GetEnumerator()
        {
            return Child.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
