using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class VariantDeclaration : VariantSymbol
    {
        public TupleLiteral AttributeAccess { get; private set; }
        public Identifier Ident { get; private set; }
        public Identifier ExplicitType { get; private set; }

        public VariantDeclaration(TextPosition tp, VariantType type, TupleLiteral attr, Identifier ident, Identifier expli, Element def = null)
            : base(tp, type, def)
        {
            AttributeAccess = attr;
            Ident = ident;
            ExplicitType = expli;
            Name = Ident == null ? string.Empty : Ident.Value;
            AppendChild(AttributeAccess);
            AppendChild(Ident);
            AppendChild(ExplicitType);
        }

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                var a = new List<AttributeSymbol>();
                foreach (var v in AttributeAccess)
                {
                    a.Add(v.OverLoad.FindAttribute());
                }
                if (!a.HasAnyAttribute(AttributeType.Public, AttributeType.Protected, AttributeType.Private))
                {
                    var p = NameResolution("public").FindAttribute();
                    a.Add(p);
                }
                _Attribute = a;
                return _Attribute;
            }
        }

        public override TypeSymbol DataType
        {
            get
            {
                if(_DataType != null)
                {
                    return _DataType;
                }
                _DataType = Root.Unknown;
                var caller = Parent as CallExpression;
                if (ExplicitType != null)
                {
                    _DataType = ExplicitType.OverLoad.FindDataType().Type;
                }
                else if(caller != null && caller.HasCallTarget(this))
                {
                    _DataType = caller.CallType;
                }
                else if (DefaultValue != null)
                {
                    _DataType = DefaultValue.ReturnType;
                }
                return _DataType;
            }
        }
    }
}
