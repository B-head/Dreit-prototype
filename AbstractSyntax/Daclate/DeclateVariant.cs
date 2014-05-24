using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateVariant : VariantSymbol, IAccess
    {
        public IdentifierAccess Ident { get; set; }
        public Element ExplicitType { get; set; }

        public override DataType DataType
        {
            get
            {
                if(_DataType != null)
                {
                    return _DataType;
                }
                 var caller = Parent as ICaller;
                if (ExplicitType != null)
                {
                    _DataType = ExplicitType.DataType;
                }
                else if(caller != null && caller.HasCallTarget(this))
                {
                    _DataType = caller.GetCallType();
                }
                else
                {
                    _DataType = Root.Unknown;
                }
                return _DataType;
            }
        }

        public OverLoad Reference
        {
            get { return Ident.Reference; }
        }

        public void RefarenceResolution(Scope scope)
        {
            Ident.RefarenceResolution(scope);
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Ident;
                    case 1: return ExplicitType;
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
