using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer))]
    [Serializable]
    public class VoidScope : DataType
    {
        public VoidScope()
        {
            Name = "void";
        }
    }
}
