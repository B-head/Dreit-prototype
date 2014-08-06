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
