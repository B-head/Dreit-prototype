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

        protected override string ElementInfo
        {
            get
            {
                if (Parameters.Count == 0)
                {
                    return string.Format("{0}", Type.Name);
                }
                else
                {
                    return string.Format("{0}!({1})", Type.Name, Parameters.ToNames());
                }
            }
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

        internal override IEnumerable<OverLoadMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            var newInst = SyntaxUtility.MakeGenericInstance(Type.Generics, Parameters);
            var aftInst = newInst.Concat(inst).ToList();
            foreach (var v in Type.GetTypeMatch(aftInst, pars, args))
            {
                yield return v;
            }
        }

        internal override IEnumerable<OverLoadMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            if (Type == Root.Typeof)
            {
                foreach (var v in Parameters[0].GetTypeMatch(inst, pars, args))
                {
                    yield return v;
                }
            }
            else
            {
                var newInst = SyntaxUtility.MakeGenericInstance(Type.Generics, Parameters);
                var aftInst = newInst.Concat(inst).ToList();
                foreach (var v in Type.GetInstanceMatch(aftInst, pars, args))
                {
                    yield return v;
                }
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
