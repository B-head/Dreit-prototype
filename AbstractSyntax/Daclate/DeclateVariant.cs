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
        public TupleList AttributeAccess { get; set; }
        public IdentifierAccess Ident { get; set; }
        public Element ExplicitType { get; set; }

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
                    var acs = v as IAccess;
                    if (acs != null)
                    {
                        _Attribute.Add(acs.Reference.SelectPlain());
                    }
                }
                return _Attribute;
            }
        }

        public override IDataType DataType
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

        public Scope CallScope
        {
            get { return Ident.CallScope; }
        }

        public OverLoad Reference
        {
            get { return Ident.Reference; }
        }

        public void RefarenceResolution()
        {
            RefarenceResolution(CurrentIScope);
        }

        public void RefarenceResolution(IScope scope)
        {
            Ident.RefarenceResolution(scope);
            var etacs = ExplicitType as IAccess;
            if(etacs != null)
            {
                etacs.RefarenceResolution(scope);
            }
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
