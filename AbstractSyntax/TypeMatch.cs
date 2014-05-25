using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public struct TypeMatch
    {
        public Scope Call { get; private set; }
        public TypeMatchResult Result { get; private set; }
        public IReadOnlyList<IDataType> Actual { get; private set; }
        public IReadOnlyList<IDataType> Formal { get; private set; }
        public IReadOnlyList<IScope> Converters { get; private set; }

        internal static TypeMatch MakeTypeMatch(ConversionManager manager, Scope call, IReadOnlyList<IDataType> actual, IReadOnlyList<IDataType> formal, IReadOnlyList<GenericSymbol> generic)
        {
            return MakeTypeMatch(manager, call, actual, formal);
        }

        //todo さらに詳しい順位付けをする。
        internal static TypeMatch MakeTypeMatch(ConversionManager manager, Scope call, IReadOnlyList<IDataType> actual, IReadOnlyList<IDataType> formal)
        {
            TypeMatch result = new TypeMatch()
            {
                Call = call,
                Actual = actual,
                Formal = formal,
            };
            if(formal.Count != actual.Count)
            {
                result.Result = TypeMatchResult.MissMatchCount;
                return result;
            }
            var convs = new List<Scope>();
            for (int i = 0; i < formal.Count; i++)
            {
                var c = manager.Find(actual[i], formal[i]);
                convs.Add(c);
            }
            result.Result = TypeMatchResult.PerfectMatch;
            result.Converters = convs;
            foreach (var v in convs)
            {
                if (v is VoidSymbol)
                {
                    continue;
                }
                else if (v is ErrorSymbol)
                {
                    result.Result = TypeMatchResult.MissMatchType;
                    break;
                }
                else if (v is UnknownSymbol)
                {
                    result.Result = TypeMatchResult.Unknown;
                }
                else if(result.Result != TypeMatchResult.Unknown)
                {
                    result.Result = TypeMatchResult.ConvertMatch;
                }
            }
            return result;
        }

        internal static TypeMatch MakeNotCallable(Scope call)
        {
            return new TypeMatch { Call = call, Result = TypeMatchResult.NotCallable };
        }
    }

    public enum TypeMatchResult
    {
        Unknown,
        PerfectMatch,
        ConvertMatch,
        NotCallable,
        MissMatchCount,
        MissMatchType,
    }
}
