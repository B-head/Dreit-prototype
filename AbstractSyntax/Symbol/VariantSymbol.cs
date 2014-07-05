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
        protected List<Scope> _Attribute;
        protected Scope _ReturnType;

        protected VariantSymbol(TextPosition tp)
            : base(tp)
        {

        }

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute; }
        }

        public override Scope ReturnType
        {
            get { return _ReturnType; }
        }

        public override Scope CallReturnType
        {
            get { return ReturnType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new Scope[] { ReturnType });
        }

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
            if (ReturnType is UnknownSymbol)
            {
                CompileError("require-type");
            }
        }
    }
}
