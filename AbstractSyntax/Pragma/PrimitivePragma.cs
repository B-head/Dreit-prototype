using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Pragma
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer))]
    [Serializable]
    public class PrimitivePragma : DeclateClass
    {
        public PrimitivePragmaType Type { get; private set; }

        public PrimitivePragma(PrimitivePragmaType type)
        {
            Type = type;
        }

        public override bool IsPragma
        {
            get { return true; }
        }
    }

    public enum PrimitivePragmaType
    {
        NotPrimitive,
        Root,
        Boolean,
        Integer8,
        Integer16,
        Integer32,
        Integer64,
        Natural8,
        Natural16,
        Natural32,
        Natural64,
        Binary32,
        Binary64,
    }
}
