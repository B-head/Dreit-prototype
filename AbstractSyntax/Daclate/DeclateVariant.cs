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
                var a = new List<Scope>();
                foreach (var v in AttributeAccess)
                {
                    a.Add(v.OverLoad.FindDataType());
                }
                if(IsLet)
                {
                    a.Add(Root.Let);
                }
                else
                {
                    a.Add(Root.Var);
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
                var caller = Parent as CallRoutine;
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
