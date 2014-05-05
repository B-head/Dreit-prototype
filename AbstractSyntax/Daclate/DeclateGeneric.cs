using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateGeneric : DataType
    {
        public IdentifierAccess Ident { get; set; }
        public Element SpecializationType { get; set; }

        public override int Count
        {
            get { return 1; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Ident;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void SpreadElement(Element parent, Scope scope)
        {
            Name = Ident == null ? string.Empty : Ident.Value;
            base.SpreadElement(parent, scope);
        }
    }
}
