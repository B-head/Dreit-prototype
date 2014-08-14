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
                _Attribute = a;
                return _Attribute;
            }
        }

        public override Scope CallReturnType
        {
            get
            {
                if (_DataType != null)
                {
                    return _DataType;
                }
                if (ExplicitType != null)
                {
                    _DataType = ExplicitType.OverLoad.FindDataType();
                }
                else if (DefaultValue != null)
                {
                    _DataType = DefaultValue.ReturnType;
                }
                else
                {
                    _DataType = Root.ErrorType;
                }
                return _DataType;
            }
        }
    }
}
