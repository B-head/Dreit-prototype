using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer))]
    [Serializable]
    public abstract class CilStructure : IReadOnlyTree<CilStructure>
    {
        private RootStructure _Root;
        private List<CilStructure> Child;
        public CilStructure Parent { get; private set; }
        private bool IsBuilded;

        protected CilStructure()
        {
            Child = new List<CilStructure>();
            var root = this as RootStructure;
            if (root != null)
            {
                _Root = root;
            }
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

        internal void AppendChild(IEnumerable<CilStructure> childs)
        {
            foreach(var v in childs)
            {
                AppendChild(v);
            }
        }

        private void RegisterParent(CilStructure parent)
        {
            if(Parent != null)
            {
                throw new InvalidOperationException();
            }
            Parent = parent;
        }

        internal void RelayBuildCode()
        {
            if(IsBuilded)
            {
                return;
            }
            IsBuilded = true;
            BuildCode();
        }

        protected virtual void BuildCode()
        {
            return;
        }

        public RootStructure Root
        {
            get
            {
                if (_Root == null)
                {
                    _Root = Parent.Root;
                }
                return _Root;
            }
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

        CilStructure IReadOnlyTree<CilStructure>.Root
        {
            get { return Root; }
        }
    }
}
