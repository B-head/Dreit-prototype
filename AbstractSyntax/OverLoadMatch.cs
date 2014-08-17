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
        internal static OverLoadMatch MakeOverLoadMatch(Root root, RoutineSymbol call, IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ParameterSymbol> fa,
            IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa)
        {
            var ig = new List<TypeSymbol>();
            var ia = new List<TypeSymbol>();
            var convs = new List<RoutineSymbol>();
            OverLoadMatch result = new OverLoadMatch()
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
            var tgi = InferInstance(root, inst, ag, aa, ig, ia);
            result.Call = GenericsInstance.MakeRoutineTemplateInstance(root, tgi, call);
            for (int i = 0; i < ia.Count; i++)
            {
                var c = root.ConvManager.Find(aa[i], ia[i]);
                convs.Add(c);
            }
            result.Result = CheckConverterResult(convs);
            return result;
        }

        private static bool ContainGenericCount(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<TypeSymbol> ag)
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

        private static bool ContainArgumentCount(IReadOnlyList<ParameterSymbol> fa, IReadOnlyList<TypeSymbol> aa)
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

        private static bool ContainTupleCount(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ParameterSymbol> fa, IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa)
        {
            if (!HasVariadic(fg) || fg.Count > ag.Count)
            {
                return true;
            }
            return ag.Count - fg.Count == aa.Count - fa.Count;
        }

        private static void InitInstance(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<ParameterSymbol> fa,
            IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa, List<TypeSymbol> ig, List<TypeSymbol> ia)
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

        private static IReadOnlyList<GenericsInstance> InferInstance(Root root, IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> ag, IReadOnlyList<TypeSymbol> aa, List<TypeSymbol> ig, List<TypeSymbol> ia)
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

        private static bool HasVariadic(IReadOnlyList<Scope> f)
        {
            if(f.Count == 0)
            {
                return false;
            }
            return f.Last().Attribute.HasAnyAttribute(AttributeType.Variadic);
        }

        private static TypeSymbol GetVariadicType(List<TypeSymbol> ia)
        {
            var t = (ClassTemplateInstance)ia.Last();
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
                ret.Add(new GenericSymbol("@@T" + (i + 1), new List<AttributeSymbol>(), new List<Scope>()));
            }
            return ret;
        }

        private static IReadOnlyList<TypeSymbol> MakeArgument(int count, TypeSymbol scope)
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

        private static TypeMatchResult CheckConverterResult(IReadOnlyList<RoutineSymbol> convs)
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
