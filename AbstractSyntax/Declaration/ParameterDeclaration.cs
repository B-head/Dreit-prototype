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
    public class ParameterDeclaration : ParameterSymbol
    {
        public TupleLiteral AttributeAccess { get; private set; }
        public Identifier ExplicitType { get; private set; }

        public ParameterDeclaration(TextPosition tp, VariantType type, string name, TupleLiteral attr, Identifier expli, Element def)
            : base(tp, type, def)
        {
            Name = name;
            AttributeAccess = attr;
            ExplicitType = expli;
            AppendChild(AttributeAccess);
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
                _Attribute = a;
                return _Attribute;
            }
        }

        public override TypeSymbol DataType
        {
            get
            {
                if (_DataType != null)
                {
                    return _DataType;
                }
                _DataType = Root.Unknown;
                if (ExplicitType != null)
                {
                    _DataType = ExplicitType.OverLoad.FindDataType().Type;
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
