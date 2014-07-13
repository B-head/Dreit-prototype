using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateArgument : ArgumentSymbol
    {
        public TupleList AttributeAccess { get; private set; }
        public IdentifierAccess Ident { get; private set; }
        public IdentifierAccess ExplicitType { get; private set; }

        public DeclateArgument(TextPosition tp, TupleList attr, IdentifierAccess ident, IdentifierAccess expl)
            : base(tp)
        {
            AttributeAccess = attr;
            Ident = ident;
            ExplicitType = expl;
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
                _Attribute = a;
                return _Attribute;
            }
        }

        public override Scope ReturnType
        {
            get
            {
                if (_ReturnType != null)
                {
                    return _ReturnType;
                }
                var caller = Parent as CallRoutine;
                if (ExplicitType != null)
                {
                    _ReturnType = ExplicitType.OverLoad.FindDataType();
                }
                else if (caller != null && caller.HasCallTarget(this))
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

        public override OverLoadReference OverLoad
        {
            get { return Ident.OverLoad; }
        }
    }
}
