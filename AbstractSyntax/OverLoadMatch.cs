using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public struct OverLoadMatch
    {
        public Scope Call { get; private set; }
        public TypeMatchResult Result { get; private set; }
        public IReadOnlyList<GenericSymbol> FormalGenerics { get; private set; }
        public IReadOnlyList<ParameterSymbol> FormalArguments { get; private set; }
        public IReadOnlyList<Scope> ActualGenerics { get; private set; }
        public IReadOnlyList<Scope> ActualArguments { get; private set; }
        public IReadOnlyList<Scope> InstanceGenerics { get; private set; }
        public IReadOnlyList<Scope> InstanceArguments { get; private set; }
        public IReadOnlyList<Scope> Converters { get; private set; }

        //todo さらに詳しい順位付けをする。
        //todo 可変長引数とデフォルト引数に対応する。
        internal static OverLoadMatch MakeOverLoadMatch(ConversionManager manager, Scope call,
            IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ParameterSymbol> fa, IReadOnlyList<Scope> ag, IReadOnlyList<Scope> aa)
        {
            var ig = new List<Scope>();
            var ia = new List<Scope>();
            var convs = new List<Scope>();
            OverLoadMatch result = new OverLoadMatch()
            {
                Call = call,
                FormalGenerics = fg,
                FormalArguments = fa,
                ActualGenerics = ag,
                ActualArguments = aa,
                InstanceGenerics = ig,
                InstanceArguments = ia,
                Converters = convs,
            };
            if(ag.Count < fg.Count)
            {
                result.Result = TypeMatchResult.UnmatchParameterCount;
                return result;
            }
            if(fa.Count != aa.Count)
            {
                result.Result = TypeMatchResult.UnmatchArgumentCount;
                return result;
            }
            MakeInstance(fg, fa, ag, aa, ig, ia);
            for (int i = 0; i < fa.Count; i++)
            {
                var c = manager.Find(aa[i], ia[i]);
                convs.Add(c);
            }
            result.Result = CheckConverterResult(convs);
            return result;
        }

        private static void MakeInstance(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ParameterSymbol> fa, 
            IReadOnlyList<Scope> ag, IReadOnlyList<Scope> aa, List<Scope> ig, List<Scope> ia)
        {
            ig.AddRange(fg);
            ia.AddRange(fa.GetDataTypes());
            for(var i = 0; i < ag.Count; ++i)
            {
                ig[i] = ag[i];
            }
            for(var i = 0; i < ia.Count; ++i)
            {
                if(!(ia[i] is GenericSymbol))
                {
                    continue;
                }
                var k = FindIndex(fg, ia[i]);
                if(ig[k] is GenericSymbol)
                {
                    ig[k] = aa[i]; //todo 処理の順序で結果が変わるバグに対処する。
                }
                ia[i] = ig[k];
            }
        }

        private static int FindIndex(IReadOnlyList<Scope> list, Scope value)
        {
            for(var i = 0; i < list.Count; ++i)
            {
                if(list[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        private static TypeMatchResult CheckConverterResult(IReadOnlyList<Scope> convs)
        {
            var result = TypeMatchResult.PerfectMatch;
            foreach (var v in convs)
            {
                if (v is VoidSymbol)
                {
                    continue;
                }
                else if (v is ErrorSymbol)
                {
                    result = TypeMatchResult.UnmatchArgumentType;
                    break;
                }
                else if (v is UnknownSymbol)
                {
                    result = TypeMatchResult.Unknown;
                }
                else if (result != TypeMatchResult.Unknown)
                {
                    result = TypeMatchResult.ConvertMatch;
                }
            }
            return result;
        }

        internal static OverLoadMatch MakeNotCallable(Scope call)
        {
            return new OverLoadMatch { Call = call, Result = TypeMatchResult.NotCallable };
        }
    }

    public enum TypeMatchResult
    {
        Unknown,
        PerfectMatch,
        ConvertMatch,
        AmbiguityMatch,
        NotCallable,
        UnmatchArgumentCount,
        UnmatchArgumentType,
        UnmatchParameterCount,
        UnmatchParameterType,
    }
}
