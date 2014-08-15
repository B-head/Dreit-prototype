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

        public VariantDeclaration(TextPosition tp, VariantType type, TupleLiteral attr, Identifier ident, Identifier expli)
            : base(tp, type)
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
                _DataType = Root.ErrorType;
                var caller = Parent as CallExpression;
                if (ExplicitType != null)
                {
                    _DataType = ExplicitType.OverLoad.FindDataType();
                }
                else if(caller != null && caller.HasCallTarget(this))
                {
                    _DataType = caller.CallType;
                }
                return _DataType;
            }
        }

        public override OverLoad OverLoad
        {
            get { return Ident.OverLoad; }
        }
    }
}
