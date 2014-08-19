using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer), typeof(SyntaxVisualizerSource))]
    [Serializable]
    public abstract class CilStructure : IReadOnlyTree<CilStructure>
    {
        private RootStructure _Root;
        private List<CilStructure> Child;
        public CilStructure Parent { get; private set; }

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

        internal void ChildBuildCode(CilStructure stru)
        {
            foreach(var v in this)
            {
                v.BuildCode();
            }
        }

        protected void PopBuildCode(CilStructure cil)
        {
            cil.BuildCode();
            var exp = cil as ExpressionStructure;
            if(exp == null)
            {
                return;
            }
            if(!exp.ResultType.IsVoid && !CurrentContainer.IsDataTypeContext)
            {
                var cg = exp.CurrentContainer.GainGenerator();
                cg.GenerateControl(OpCodes.Pop);
            }
        }

        internal virtual void BuildCode()
        {
            ChildBuildCode(this);
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

        public ContainerStructure CurrentContainer
        {
            get
            {
                var c = Parent as ContainerStructure;
                if (c != null)
                {
                    return c;
                }
                else if (Parent != null)
                {
                    return Parent.CurrentContainer;
                }
                else
                {
                    return null;
                }
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
