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

        public override bool IsConstant
        {
            get { return true; }
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                var a = new List<Scope>();
                foreach (var v in AttributeAccess)
                {
                    a.Add(v.OverLoad.FindDataType());
                }
                if (!HasAnyAttribute(a, AttributeType.Public, AttributeType.Protected, AttributeType.Private))
                {
                    var p = NameResolution("public").FindDataType();
                    a.Add(p);
                }
                _Attribute = a;
                return _Attribute;
            }
        }

        public override Scope CallReturnType
        {
            get
            {
                if(_DataType != null)
                {
                    return _DataType;
                }
                var caller = Parent as CallExpression;
                if (ExplicitType != null)
                {
                    _DataType = ExplicitType.OverLoad.FindDataType();
                }
                else if(caller != null && caller.HasCallTarget(this))
                {
                    _DataType = caller.CallType;
                }
                else
                {
                    _DataType = Root.Unknown;
                }
                return _DataType;
            }
        }

        public override OverLoadReference OverLoad
        {
            get { return Ident.OverLoad; }
        }
    }
}
