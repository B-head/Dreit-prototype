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
    public struct TypeMatch
    {
        public Scope Call { get; private set; }
        public TypeMatchResult Result { get; private set; }
        public IReadOnlyList<Scope> ActualParameters { get; private set; }
        public IReadOnlyList<GenericSymbol> FormalParameters { get; private set; }
        public IReadOnlyList<Scope> InstanceParameters { get; private set; }
        public IReadOnlyList<Scope> ActualArguments { get; private set; }
        public IReadOnlyList<Scope> FormalArguments { get; private set; }
        public IReadOnlyList<Scope> InstanceArguments { get; private set; }
        public IReadOnlyList<Scope> Converters { get; private set; }

        //todo さらに詳しい順位付けをする。
        internal static TypeMatch MakeTypeMatch(ConversionManager manager, Scope call,
            IReadOnlyList<Scope> ap, IReadOnlyList<GenericSymbol> fp, IReadOnlyList<Scope> aa, IReadOnlyList<Scope> fa)
        {
            var ip = new List<Scope>();
            var ia = new List<Scope>();
            var convs = new List<Scope>();
            TypeMatch result = new TypeMatch()
            {
                Call = call,
                ActualParameters = ap,
                FormalParameters = fp,
                InstanceParameters = ip,
                ActualArguments = aa,
                FormalArguments = fa,
                InstanceArguments = ia,
                Converters = convs,
            };
            if(ap.Count < fp.Count)
            {
                result.Result = TypeMatchResult.UnmatchParameterCount;
                return result;
            }
            if(fa.Count != aa.Count)
            {
                result.Result = TypeMatchResult.UnmatchArgumentCount;
                return result;
            }
            MakeInstance(ap, fp, ip, aa, fa, ia);
            for (int i = 0; i < fa.Count; i++)
            {
                var c = manager.Find(aa[i], ia[i]);
                convs.Add(c);
            }
            result.Result = CheckConverterResult(convs);
            return result;
        }

        private static void MakeInstance(IReadOnlyList<Scope> ap, IReadOnlyList<GenericSymbol> fp, List<Scope> ip,
            IReadOnlyList<Scope> aa, IReadOnlyList<Scope> fa, List<Scope> ia)
        {
            ip.AddRange(fp);
            ia.AddRange(fa);
            for(var i = 0; i < ap.Count; ++i)
            {
                ip[i] = ap[i];
            }
            for(var i = 0; i < ia.Count; ++i)
            {
                if(!(ia[i] is GenericSymbol))
                {
                    continue;
                }
                var k = FindIndex(fp, ia[i]);
                if(ip[k] is GenericSymbol)
                {
                    ip[k] = aa[i]; //todo 処理の順序で結果が変わるバグに対処する。
                }
                ia[i] = ip[k];
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
        AmbiguityMatch,
        NotCallable,
        UnmatchArgumentCount,
        UnmatchArgumentType,
        UnmatchParameterCount,
        UnmatchParameterType,
    }
}
