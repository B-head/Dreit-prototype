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
        protected List<IScope> _Attribute;
        protected IDataType _ReturnType;

        public override IReadOnlyList<IScope> Attribute
        {
            get { return _Attribute; }
        }

        public override IDataType ReturnType
        {
            get { return _ReturnType; }
        }

        public override IDataType CallReturnType
        {
            get { return ReturnType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { });
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, new IDataType[] { ReturnType });
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
