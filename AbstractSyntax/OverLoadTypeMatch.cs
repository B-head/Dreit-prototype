using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public struct OverLoadTypeMatch
    {
        public TypeSymbol Type { get; private set; }
        public TypeMatchResult Result { get; private set; }
        public IReadOnlyList<GenericSymbol> FormalGenerics { get; private set; }
        public IReadOnlyList<GenericsInstance> ScopeInstance { get; private set; }
        public IReadOnlyList<TypeSymbol> ActualGenerics { get; private set; }
        public IReadOnlyList<TypeSymbol> InstanceGenerics { get; private set; }


        internal static OverLoadTypeMatch MakeNotType(TypeSymbol type)
        {
            return new OverLoadTypeMatch { Type = type, Result = TypeMatchResult.NotType };
        }

        internal static OverLoadTypeMatch MakeUnknown(TypeSymbol type)
        {
            return new OverLoadTypeMatch { Type = type, Result = TypeMatchResult.Unknown };
        }

        internal static OverLoadTypeMatch MakeMatch(Root root, TypeSymbol type, IReadOnlyList<GenericSymbol> fg, IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> ag)
        {
            var ig = new List<TypeSymbol>();
            var result = new OverLoadTypeMatch()
            {
                Type = type,
                FormalGenerics = fg,
                ScopeInstance = inst,
                ActualGenerics = ag,
                InstanceGenerics = ig,
            };
            if (TypeSymbol.HasAnyErrorType(fg))
            {
                result.Result = TypeMatchResult.Unknown;
                return result;
            }
            if (!ContainGenericCount(fg, ag))
            {
                result.Result = TypeMatchResult.UnmatchGenericCount;
                return result;
            }
            InitInstance(fg, ag, ig);
            var tgi = InferInstance(root, inst, ag, ig);
            if (TypeSymbol.HasAnyErrorType(tgi))
            {
                result.Result = TypeMatchResult.UnmatchGenericType;
                return result;
            }
            result.Result = TypeMatchResult.PerfectMatch;
            result.Type = GenericsInstance.MakeClassTemplateInstance(root, tgi, type);
            return result;
        }

        internal static bool ContainGenericCount(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<TypeSymbol> ag)
        {
            if (ArgumentSymbol.HasVariadic(fg))
            {
                return true;
            }
            else
            {
                return fg.Count >= ag.Count;
            }
        }

        private static void InitInstance(IReadOnlyList<GenericSymbol> fg, IReadOnlyList<TypeSymbol> ag, List<TypeSymbol> ig)
        {
            if (!ArgumentSymbol.HasVariadic(fg))
            {
                ig.AddRange(fg);
            }
            else
            {
                ig.AddRange(fg);
                ig.RemoveAt(ig.Count - 1);
                var c = (ag.Count - fg.Count + 1);
                var mg = MakeGeneric(c);
                ig.AddRange(mg);
            }
        }

        private static IReadOnlyList<GenericsInstance> InferInstance(Root root, IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> ag, List<TypeSymbol> ig)
        {
            var tgi = new List<GenericsInstance>();
            for (var i = 0; i < ig.Count; ++i)
            {
                if (i < ag.Count)
                {
                    tgi.Add(new GenericsInstance { Generic = (GenericSymbol)ig[i], Type = ag[i] });
                }
                else
                {
                    tgi.Add(new GenericsInstance { Generic = (GenericSymbol)ig[i], Type = root.Unknown });
                }
            }
            tgi = tgi.Concat(inst).ToList();
            for (var i = 0; i < ig.Count; ++i)
            {
                ig[i] = GenericsInstance.MakeClassTemplateInstance(root, tgi, ig[i]);
            }
            return tgi;
        }

        internal static IReadOnlyList<GenericSymbol> MakeGeneric(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }
            var ret = new List<GenericSymbol>();
            for (var i = 0; i < count; ++i)
            {
                ret.Add(new GenericSymbol("@@T" + (i + 1), new List<AttributeSymbol>(), new List<Scope>()));
            }
            return ret;
        }
    }

    public enum TypeMatchResult
    {
        Unknown,
        NotType,
        PerfectMatch,
        ConvertMatch,
        AmbiguityMatch,
        UnmatchGenericCount,
        UnmatchGenericType,
    }
}
