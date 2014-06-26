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
        public TupleList Attribute { get; set; }
        public IdentifierAccess Ident { get; set; }
        public Element ExplicitType { get; set; }

        public override IDataType DataType
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

        public void RefarenceResolution(IScope scope)
        {
            Ident.RefarenceResolution(scope);
        }

        public override int Count
        {
            get { return 3; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Attribute;
                    case 1: return Ident;
                    case 2: return ExplicitType;
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
