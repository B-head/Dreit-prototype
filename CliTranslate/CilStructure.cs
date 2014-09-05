/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
                cg.GenerateCode(OpCodes.Pop);
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
