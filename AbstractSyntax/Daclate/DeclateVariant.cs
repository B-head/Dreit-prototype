using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateVariant : VariantSymbol
    {
        public TupleList AttributeAccess { get; set; }
        public IdentifierAccess Ident { get; set; }
        public IdentifierAccess ExplicitType { get; set; }

        public override IReadOnlyList<IScope> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                _Attribute = new List<IScope>();
                foreach (var v in AttributeAccess)
                {
                    _Attribute.Add(v.Reference.GetDataType());
                }
                return _Attribute;
            }
        }

        public override IDataType ReturnType
        {
            get
            {
                if(_DataType != null)
                {
                    return _DataType;
                }
                var caller = Parent as Caller;
                if (ExplicitType != null)
                {
                    _DataType = ExplicitType.Reference.GetDataType();
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

        public override OverLoad Reference
        {
            get { return Ident.Reference; }
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
                    case 0: return AttributeAccess;
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
