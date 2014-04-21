using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer))]
    [Serializable]
    public class ThisScope : Scope
    {
        private DeclateClass _DataType;

        public ThisScope(DeclateClass dataType)
        {
            Name = "this";
            _DataType = dataType;
        }

        public override DataType DataType
        {
            get { return _DataType; }
        }
    }
}
