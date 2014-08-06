using AbstractSyntax.Pragma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class VariantSymbol : Scope
    {
        public bool IsLet { get; private set; }
        public PropertyPragma Getter { get; private set; }
        public PropertyPragma Setter { get; private set; }
        protected IReadOnlyList<Scope> _Attribute;
        protected Scope _DataType;

        protected VariantSymbol()
        {
            Getter = new PropertyPragma(this, false);
            Setter = new PropertyPragma(this, true);
            AppendChild(Getter);
            AppendChild(Setter);
        }

        protected VariantSymbol(TextPosition tp, bool isLet)
            : base(tp)
        {
            IsLet = isLet;
            Getter = new PropertyPragma(this, false);
            Setter = new PropertyPragma(this, true);
            AppendChild(Getter);
            AppendChild(Setter);
        }

        public VariantSymbol(string name, bool isLet, IReadOnlyList<Scope> attr, Scope dt)
        {
            Name = name;
            IsLet = isLet;
            _Attribute = attr;
            _DataType = dt;
            Getter = new PropertyPragma(this, false);
            Setter = new PropertyPragma(this, true);
            AppendChild(Getter);
            AppendChild(Setter);
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute; }
        }

        public override Scope ReturnType
        {
            get { return CallReturnType; }
        }

        public override Scope CallReturnType
        {
            get { return _DataType; }
        }

        public bool IsField
        {
            get { return CurrentScope is ClassSymbol; }
        }

        public bool IsLocal
        {
            get { return CurrentScope is RoutineSymbol; }
        }

        public bool IsGlobal
        {
            get { return CurrentScope is NameSpaceSymbol; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            foreach (var v in Getter.GetTypeMatch(pars, args))
            {
                yield return v;
            }
            foreach (var v in Setter.GetTypeMatch(pars, args))
            {
                yield return v;
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (ReturnType is UnknownSymbol)
            {
                cmm.CompileError("require-type", this);
            }
        }
    }
}
