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
        public Element ExplicitType { get; private set; }

        public VariantDeclaration(TextPosition tp, VariantType type, string name, TupleLiteral attr, Element expli, Element def = null)
            : base(tp, type, def)
        {
            Name = name;
            AttributeAccess = attr;
            ExplicitType = expli;
            AppendChild(AttributeAccess);
            AppendChild(ExplicitType);
        }

        internal override void Prepare()
        {
            if (DefaultValue != null)
            {
                return;
            }
            var caller = Parent as CallExpression;
            if (caller == null)
            {
                return;
            }
            DefaultValue = caller.CallValue;
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
                if (VariantType == VariantType.Const)
                {
                    var p = NameResolution("static").FindAttribute();
                    a.Add(p);
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
