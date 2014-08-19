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
    public struct OverLoadCallMatch
    {
        public RoutineSymbol Call { get; private set; }
        public CallMatchResult Result { get; private set; }
        public IReadOnlyList<GenericSymbol> FormalGenerics { get; private set; }
        public IReadOnlyList<ArgumentSymbol> FormalArguments { get; private set; }
        public IReadOnlyList<GenericsInstance> ScopeInstance { get; private set; }
        public IReadOnlyList<TypeSymbol> ActualGenerics { get; private set; }
        public IReadOnlyList<TypeSymbol> ActualArguments { get; private set; }
        public IReadOnlyList<TypeSymbol> InstanceGenerics { get; private set; }
        public IReadOnlyList<TypeSymbol> InstanceArguments { get; private set; }
        public IReadOnlyList<RoutineSymbol> Converters { get; private set; }

        public override string ToString()
        {
            return string.Format("Result = {0}, Call = {{1}}", Result, Call);
        }

        internal static OverLoadCallMatch MakeNotCallable(RoutineSymbol call)
        {
            return new OverLoadCallMatch { Call = call, Result = CallMatchResult.NotCallable };
        }

        internal static OverLoadCallMatch MakeUnknown(RoutineSymbol call)
        {
            return new OverLoadCallMatch { Call = call, Result = CallMatchResult.Unknown };
        }

        //todo さらに詳しい順位付けをする。
        //todo デフォルト引数に対応する。
        //todo 型制約に対応する。
        internal static OverLoadCallMatch MakeMatch(Root root, RoutineSymbol call, IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ArgumentSymbol> fa,
            IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa)
        {
            var ig = new List<TypeSymbol>();
            var ia = new List<TypeSymbol>();
            var convs = new List<RoutineSymbol>();
            var result = new OverLoadCallMatch()
            {
                Call = call,
                FormalGenerics = fg,
                FormalArguments = fa,
                ScopeInstance = inst,
                ActualGenerics = ag,
                ActualArguments = aa,
                InstanceGenerics = ig,
                InstanceArguments = ia,
                Converters = convs,
            };
            if (TypeSymbol.HasAnyErrorType(fg) || TypeSymbol.HasAnyErrorType(fa.GetDataTypes()))
            {
                result.Result = CallMatchResult.Unknown;
                return result;
            }
            if (!OverLoadTypeMatch.ContainGenericCount(fg, ag))
            {
                result.Result = CallMatchResult.UnmatchGenericCount;
                return result;
            }
            if (!ContainArgumentCount(fa, aa) || !ContainTupleCount(fg, fa, ag, aa))
            {
                result.Result = CallMatchResult.UnmatchArgumentCount;
                return result;
            }
            InitInstance(fg, fa, ag, aa, ig, ia);
            var tgi = InferInstance(root, inst, ag, aa, ig, ia);
            if (TypeSymbol.HasAnyErrorType(tgi))
            {
                result.Result = CallMatchResult.UnmatchGenericType;
                return result;
            }
            for (int i = 0; i < ia.Count; i++)
            {
                var c = root.ConvManager.Find(aa[i], ia[i]);
                convs.Add(c);
            }
            result.Result = CheckConverterResult(convs);
            if (HasMatch(result.Result))
            {
                result.Call = GenericsInstance.MakeRoutineTemplateInstance(root, tgi, call);
            }
            return result;
        }

        internal static bool ContainArgumentCount(IReadOnlyList<ArgumentSymbol> fa, IReadOnlyList<TypeSymbol> aa)
        {
            if (ArgumentSymbol.HasVariadic(fa))
            {
                return fa.Count - 1 <= aa.Count;
            }
            else
            {
                return fa.Count == aa.Count;
            }
        }

        internal static bool ContainTupleCount(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ArgumentSymbol> fa, IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa)
        {
            if (!ArgumentSymbol.HasVariadic(fg) || fg.Count > ag.Count)
            {
                return true;
            }
            return ag.Count - fg.Count == aa.Count - fa.Count;
        }

        private static void InitInstance(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ArgumentSymbol> fa,
            IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa, List<TypeSymbol> ig, List<TypeSymbol> ia)
        {
            if (!ArgumentSymbol.HasVariadic(fg))
            {
                ig.AddRange(fg);
                if (!ArgumentSymbol.HasVariadic(fa))
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
                var mg = OverLoadTypeMatch.MakeGeneric(c);
                ig.AddRange(mg);
                if (!ArgumentSymbol.HasVariadic(fa))
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

        private static IReadOnlyList<GenericsInstance> InferInstance(Root root, IReadOnlyList<GenericsInstance> inst,
            IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa, List<TypeSymbol> ig, List<TypeSymbol> ia)
        {
            var tgi = new List<GenericsInstance>();
            for (var i = 0; i < ig.Count; ++i)
            {
                if(i < ag.Count)
                {
                    tgi.Add(new GenericsInstance { Generic = (GenericSymbol)ig[i], Type = ag[i] });
                }
                else
                {
                    tgi.Add(new GenericsInstance { Generic = (GenericSymbol)ig[i], Type = root.Unknown });
                }
            }
            tgi = tgi.Concat(inst).ToList();
            for (var i = 0; i < ia.Count; ++i)
            {
                var g = ia[i] as GenericSymbol;
                if(g == null)
                {
                    continue;
                }
                var k = GenericsInstance.FindGenericIndex(tgi, g);
                if(k == -1)
                {
                    continue;
                }
                var gi = tgi[k];
                if (tgi[k].Type is UnknownSymbol)
                {
                    gi.Type = aa[i]; 
                }
                else
                {
                    gi.Type = GetCommonSubType(gi.Type, aa[i]); 
                }
                tgi[k] = gi;
            }
            for (var i = 0; i < ig.Count; ++i)
            {
                ig[i] = GenericsInstance.MakeClassTemplateInstance(root, tgi, ig[i]);
            }
            for (var i = 0; i < ia.Count; ++i)
            {
                ia[i] = GenericsInstance.MakeClassTemplateInstance(root, tgi, ia[i]);
            }
            return tgi;
        }

        private static TypeSymbol GetVariadicType(List<TypeSymbol> ia)
        {
            var t = (ClassTemplateInstance)ia.Last();
            return t.Parameters[0];
        }

        internal static IReadOnlyList<TypeSymbol> MakeArgument(int count, TypeSymbol scope)
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }
            var ret = new List<TypeSymbol>();
            for (var i = 0; i < count; ++i)
            {
                ret.Add(scope);
            }
            return ret;
        }

        private static TypeSymbol GetCommonSubType(TypeSymbol t1, TypeSymbol t2)
        {
            return t1; //todo 処理の順序で結果が変わるバグに対処する。共通のサブタイプを返すようにする。
        }

        private static CallMatchResult CheckConverterResult(IReadOnlyList<RoutineSymbol> convs)
        {
            var result = CallMatchResult.PerfectMatch;
            foreach (var v in convs)
            {
                if (v is DefaultSymbol)
                {
                    continue;
                }
                else if (v is ErrorRoutineSymbol)
                {
                    result = CallMatchResult.UnmatchArgumentType;
                    break;
                }
                else
                {
                    result = CallMatchResult.ConvertMatch;
                }
            }
            return result;
        }

        internal static int GetMatchPriority(CallMatchResult r)
        {
            switch (r)
            {
                case CallMatchResult.Unknown: return 10;
                case CallMatchResult.PerfectMatch: return 9;
                case CallMatchResult.ConvertMatch: return 8;
                case CallMatchResult.AmbiguityMatch: return 7;
                case CallMatchResult.UnmatchArgumentType: return 4;
                case CallMatchResult.UnmatchArgumentCount: return 3;
                case CallMatchResult.UnmatchGenericType: return 2;
                case CallMatchResult.UnmatchGenericCount: return 1;
                case CallMatchResult.NotCallable: return 0;
                default: throw new ArgumentException("r");
            }
        }

        internal static bool HasMatch(CallMatchResult r)
        {
            switch (r)
            {
                case CallMatchResult.PerfectMatch: return true;
                case CallMatchResult.ConvertMatch: return true;
                case CallMatchResult.AmbiguityMatch: return true;
                default: return false;
            }
        }
    }

    public enum CallMatchResult
    {
        Unknown,
        NotCallable,
        PerfectMatch,
        ConvertMatch,
        AmbiguityMatch,
        UnmatchArgumentCount,
        UnmatchArgumentType,
        UnmatchGenericCount,
        UnmatchGenericType,
    }
}
