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
        public RoutineSymbol Call { get; private set; }
        public TypeMatchResult Result { get; private set; }
        public IReadOnlyList<GenericSymbol> FormalGenerics { get; private set; }
        public IReadOnlyList<ParameterSymbol> FormalArguments { get; private set; }
        public IReadOnlyList<Scope> ActualGenerics { get; private set; }
        public IReadOnlyList<Scope> ActualArguments { get; private set; }
        public IReadOnlyList<Scope> InstanceGenerics { get; private set; }
        public IReadOnlyList<Scope> InstanceArguments { get; private set; }
        public IReadOnlyList<Scope> Converters { get; private set; }

        internal static OverLoadMatch MakeNotCallable(RoutineSymbol call)
        {
            return new OverLoadMatch { Call = call, Result = TypeMatchResult.NotCallable };
        }

        internal static OverLoadMatch MakeUnknown(RoutineSymbol call)
        {
            return new OverLoadMatch { Call = call, Result = TypeMatchResult.Unknown };
        }

        //todo さらに詳しい順位付けをする。
        //todo デフォルト引数に対応する。
        internal static OverLoadMatch MakeOverLoadMatch(ConversionManager manager, RoutineSymbol call,
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
            if(CheckErrorType(fg) || CheckErrorType(fa.GetDataTypes()))
            {
                result.Result = TypeMatchResult.Unknown;
                return result;
            }
            if (!ContainGenericCount(fg, ag))
            {
                result.Result = TypeMatchResult.UnmatchGenericCount;
                return result;
            }
            if (!ContainArgumentCount(fa, aa) || !ContainTupleCount(fg, fa, ag, aa))
            {
                result.Result = TypeMatchResult.UnmatchArgumentCount;
                return result;
            }
            InitInstance(fg, fa, ag, aa, ig, ia);
            InferInstance(ag, aa, ig, ia);
            for (int i = 0; i < ia.Count; i++)
            {
                var c = manager.Find(aa[i], ia[i]);
                convs.Add(c);
            }
            result.Result = CheckConverterResult(convs);
            return result;
        }

        private static bool ContainGenericCount(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<Scope> ag)
        {
            if (HasVariadic(fg))
            {
                return true;
            }
            else
            {
                return fg.Count >= ag.Count;
            }
        }

        private static bool ContainArgumentCount(IReadOnlyList<ParameterSymbol> fa, IReadOnlyList<Scope> aa)
        {
            if(HasVariadic(fa))
            {
                return fa.Count - 1 <= aa.Count;
            }
            else
            {
                return fa.Count == aa.Count;
            }
        }

        private static bool ContainTupleCount(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ParameterSymbol> fa, IReadOnlyList<Scope> ag, IReadOnlyList<Scope> aa)
        {
            if (!HasVariadic(fg) || fg.Count > ag.Count)
            {
                return true;
            }
            return ag.Count - fg.Count == aa.Count - fa.Count;
        }

        private static void InitInstance(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ParameterSymbol> fa,
            IReadOnlyList<Scope> ag, IReadOnlyList<Scope> aa, List<Scope> ig, List<Scope> ia)
        {
            if (!HasVariadic(fg))
            {
                ig.AddRange(fg);
                if (!HasVariadic(fa))
                {
                    ia.AddRange(fa.GetDataTypes());
                }
                else
                {
                    ia.AddRange(fa.GetDataTypes());
                    var t = GetVariadicType(ia);
                    ia.RemoveAt(ia.Count - 1);
                    ia.AddRange(MakeArgument(aa.Count - fa.Count + 1, t));
                }
            }
            else
            {
                ig.AddRange(fg);
                ig.RemoveAt(ig.Count - 1);
                var c = (fg.Count > ag.Count) ? (aa.Count - fa.Count + 1) : (ag.Count - fg.Count + 1);
                var mg = MakeGeneric(c);
                ig.AddRange(mg);
                if (!HasVariadic(fa))
                {
                    ia.AddRange(fa.GetDataTypes());
                }
                else
                {
                    ia.AddRange(fa.GetDataTypes());
                    ia.RemoveAt(ia.Count - 1);
                    ia.AddRange(mg);
                }
            }
        }

        private static void InferInstance(IReadOnlyList<Scope> ag, IReadOnlyList<Scope> aa, List<Scope> ig, List<Scope> ia)
        {
            var tg = new List<Scope>(ig);
            for (var i = 0; i < ag.Count; ++i)
            {
                ig[i] = ag[i];
            }
            for (var i = 0; i < ia.Count; ++i)
            {
                if(!(ia[i] is GenericSymbol))
                {
                    continue;
                }
                var k = FindTypeIndex(tg, ia[i]);
                if(ig[k] is GenericSymbol)
                {
                    ig[k] = aa[i]; 
                }
                else
                {
                    ig[k] = GetCommonSubType(ig[k], aa[i]); 
                }
            }
            for (var i = 0; i < ia.Count; ++i)
            {
                if (!(ia[i] is GenericSymbol))
                {
                    continue;
                }
                var k = FindTypeIndex(tg, ia[i]);
                ia[i] = ig[k];
            }
        }

        private static bool HasVariadic(IReadOnlyList<Scope> f)
        {
            if(f.Count == 0)
            {
                return false;
            }
            return SyntaxUtility.HasAnyAttribute(f.Last().Attribute, AttributeType.Variadic);
        }

        private static Scope GetVariadicType(List<Scope> ia)
        {
            var t = (TemplateInstanceSymbol)ia.Last();
            return t.Parameters[0];
        }

        private static IReadOnlyList<GenericSymbol> MakeGeneric(int count)
        {
            if(count < 0)
            {
                throw new ArgumentException("count");
            }
            var ret = new List<GenericSymbol>();
            for(var i = 0; i < count; ++i)
            {
                ret.Add(new GenericSymbol("@@T" + (i + 1), new List<Scope>(), new List<Scope>()));
            }
            return ret;
        }

        private static IReadOnlyList<Scope> MakeArgument(int count, Scope scope)
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }
            var ret = new List<Scope>();
            for (var i = 0; i < count; ++i)
            {
                ret.Add(scope);
            }
            return ret;
        }

        private static int FindTypeIndex(IReadOnlyList<Scope> list, Scope value)
        {
            for(var i = 0; i < list.Count; ++i)
            {
                if(list[i] == value)
                {
                    return i;
                }
            }
            throw new ArgumentException("value");
        }

        private static Scope GetCommonSubType(Scope t1, Scope t2)
        {
            return t1; //todo 処理の順序で結果が変わるバグに対処する。共通のサブタイプを返すようにする。
        }

        private static TypeMatchResult CheckConverterResult(IReadOnlyList<Scope> convs)
        {
            var result = TypeMatchResult.PerfectMatch;
            foreach (var v in convs)
            {
                if (v is DefaultSymbol)
                {
                    continue;
                }
                else if (v is ErrorRoutineSymbol)
                {
                    result = TypeMatchResult.UnmatchArgumentType;
                    break;
                }
                else
                {
                    result = TypeMatchResult.ConvertMatch;
                }
            }
            return result;
        }

        internal static bool CheckErrorType(IReadOnlyList<Scope> scope)
        {
            foreach (var v in scope)
            {
                if (v is VoidSymbol || v is UnknownSymbol || v is ErrorTypeSymbol)
                {
                    return true;
                }
            }
            return false;
        }

        internal static int GetMatchPriority(TypeMatchResult r)
        {
            switch (r)
            {
                case TypeMatchResult.Unknown: return 10;
                case TypeMatchResult.PerfectMatch: return 9;
                case TypeMatchResult.ConvertMatch: return 8;
                case TypeMatchResult.AmbiguityMatch: return 7;
                case TypeMatchResult.UnmatchArgumentType: return 4;
                case TypeMatchResult.UnmatchArgumentCount: return 3;
                case TypeMatchResult.UnmatchGenericType: return 2;
                case TypeMatchResult.UnmatchGenericCount: return 1;
                case TypeMatchResult.NotCallable: return 0;
                default: throw new ArgumentException("r");
            }
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
        UnmatchGenericCount,
        UnmatchGenericType,
    }
}
