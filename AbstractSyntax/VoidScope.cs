using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class VoidScope : DataType
    {
        public VoidScope()
        {
            Name = "void";
        }
    }
}
