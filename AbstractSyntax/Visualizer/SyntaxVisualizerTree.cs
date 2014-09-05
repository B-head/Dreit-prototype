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
using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Visualizer
{
    [Serializable]
    class SyntaxVisualizerTree : IEquatable<SyntaxVisualizerTree>
    {
        private int Id;
        private string Title;
        public bool IsTransfer { get; private set; }
        public SyntaxVisualizerTree Parent { get; private set; }
        public IReadOnlyList<SyntaxVisualizerTree> Child { get; private set; }
        public IReadOnlyDictionary<string, object> PropertyValues { get; private set; }

        public SyntaxVisualizerTree(int id, string title, SyntaxVisualizerTree parent)
        {
            Id = id;
            Title = title;
            Parent = parent;
        }

        public void TransferValues(IVisualizerObjectProvider provider)
        {
            if(IsTransfer)
            {
                return;
            }
            IsTransfer = true;
            var data = (TreeData)provider.TransferObject(Id);
            Child = data.Child;
            PropertyValues = data.PropertyValues;
        }

        public override string ToString()
        {
            return Title;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SyntaxVisualizerTree;
            if(other == null)
            {
                return false;
            }
            return Equals(other);
        }

        public bool Equals(SyntaxVisualizerTree other)
        {
            return Id == other.Id;
        }
    }
}
