using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoadSet : OverLoad
    {
        public Scope ThisScope { get; private set; }
        private List<Scope> Symbols;
        private bool IsHoldAlias;

        internal OverLoadSet(Scope scope)
        {
            ThisScope = scope;
            Symbols = new List<Scope>();
        }

        internal void Append(Scope scope)
        {
            if(scope is AliasDeclaration)
            {
                IsHoldAlias = true;
            }
            Symbols.Add(scope);
        }

        public override bool IsUndefined
        {
            get { return Symbols.Count == 0; }
        }

        internal override Root Root
        {
            get { return ThisScope.Root; }
        }

        internal override IEnumerable<Scope> TraversalChilds()
        {
            if (IsHoldAlias)
            {
                SpreadAlias();
            }
            foreach (var v in Symbols)
            {
                yield return v;
            }
        }

        internal override IEnumerable<Scope> TraversalDataType()
        {
            if(IsHoldAlias)
            {
                SpreadAlias();
            }
            foreach(var v in Symbols)
            {
                if(v.IsDataType)
                {
                    yield return v;
                }
            }
        }

        internal override IEnumerable<OverLoadMatch> TraversalCall(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            if (IsHoldAlias)
            {
                SpreadAlias();
            }
            foreach (var s in Symbols)
            {
                foreach (var m in s.GetTypeMatch(pars, args))
                {
                    yield return m;
                }
            }
        }

        private void SpreadAlias()
        {
            var alias = Symbols.FindAll(v => v is AliasDeclaration);
            Symbols.RemoveAll(v => v is AliasDeclaration);
            foreach(var v in alias)
            {
                var ol = v.OverLoad;
                foreach (var s in ol.TraversalChilds())
                {
                    Append(s);
                }
            }
        }
    }
}
