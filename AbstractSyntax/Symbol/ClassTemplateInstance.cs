using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class ClassTemplateInstance : TypeSymbol
    {
        public TypeSymbol Type { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }

        public ClassTemplateInstance(TypeSymbol type, IReadOnlyList<TypeSymbol> parameter)
        {
            Type = type;
            Parameters = parameter;
        }

        internal override OverLoadChain NameResolution(string name)
        {
            if (ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            OverLoadChain n = Root.UndefinedOverLord;
            var m = Type as ModifyTypeSymbol;
            if (m != null && ModifyTypeSymbol.HasInheritModify(m.ModifyType))
            {
                n = Parameters[0].NameResolution(name);
            }
            else
            {
                n = Type.NameResolution(name);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        internal override IEnumerable<OverLoadMatch> GetTypeMatch(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            foreach (var v in Type.GetTypeMatch(pars, args))
            {
                yield return v;
            }
        }

        internal override IEnumerable<OverLoadMatch> GetInstanceTypeMatch(IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            foreach (var v in Type.GetInstanceTypeMatch(pars, args))
            {
                yield return v;
            }
        }

        internal override IEnumerable<TypeSymbol> EnumSubType()
        {
            yield return this;
            foreach (var b in Type.EnumSubType())
            {
                yield return b;
            }
        }

        internal bool ContainClass(ClassSymbol cls)
        {
            var m = Type as ModifyTypeSymbol;
            if (m != null && ModifyTypeSymbol.HasInheritModify(m.ModifyType))
            {
                return Parameters[0] == cls;
            }
            return false;
        }
    }
}
