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
        public TupleList AttributeAccess { get; private set; }
        public IdentifierAccess Ident { get; private set; }
        public IdentifierAccess ExplicitType { get; private set; }
        public bool IsLet { get; private set; }

        public DeclateVariant(TextPosition tp, TupleList attr, IdentifierAccess ident, IdentifierAccess expl, bool isLet)
            :base(tp)
        {
            AttributeAccess = attr;
            Ident = ident;
            ExplicitType = expl;
            IsLet = isLet;
            Name = Ident == null ? string.Empty : Ident.Value;
            AppendChild(AttributeAccess);
            AppendChild(Ident);
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
                _Attribute = new List<Scope>();
                foreach (var v in AttributeAccess)
                {
                    _Attribute.Add(v.OverLoad.FindDataType());
                }
                if(IsLet)
                {
                    _Attribute.Add(Root.Let);
                }
                else
                {
                    _Attribute.Add(Root.Var);
                }
                if (!IsAnyAttribute(AttributeType.Public, AttributeType.Protected, AttributeType.Private))
                {
                    var p = NameResolution("public").FindDataType();
                    _Attribute.Add(p);
                }
                return _Attribute;
            }
        }

        public override Scope ReturnType
        {
            get
            {
                if(_ReturnType != null)
                {
                    return _ReturnType;
                }
                var caller = Parent as CallRoutine;
                if (ExplicitType != null)
                {
                    _ReturnType = ExplicitType.OverLoad.FindDataType();
                }
                else if(caller != null && caller.HasCallTarget(this))
                {
                    _ReturnType = caller.CallType;
                }
                else
                {
                    _ReturnType = Root.Unknown;
                }
                return _ReturnType;
            }
        }

        public override OverLoad OverLoad
        {
            get { return Ident.OverLoad; }
        }
    }
}
