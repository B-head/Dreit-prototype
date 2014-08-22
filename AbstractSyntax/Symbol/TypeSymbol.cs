/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public abstract class TypeSymbol : Scope
    {
        private IReadOnlyList<GenericSymbol> _TacitGeneric;
        private bool DisguiseScopeMode;

        protected TypeSymbol()
        {

        }

        protected TypeSymbol(TextPosition tp)
            : base(tp)
        {

        }

        public override TypeSymbol ReturnType
        {
            get { return Root.ClassManager.Issue(Root.Typeof, new TypeSymbol[] { this }, new TypeSymbol[0]); }
        }

        public override OverLoad OverLoad
        {
            get { return Root.SimplexManager.Issue(this); }
        }

        public override bool IsConstant
        {
            get { return true; }
        }

        public override bool IsExecutionContext
        {
            get { return false; }
        }

        public virtual IReadOnlyList<GenericSymbol> Generics
        {
            get { throw new NotImplementedException(); }
        }

        public virtual IReadOnlyList<TypeSymbol> Inherit
        {
            get { throw new NotImplementedException(); }
        }

        public IReadOnlyList<GenericSymbol> TacitGeneric
        {
            get
            {
                if (_TacitGeneric != null)
                {
                    return _TacitGeneric;
                }
                var list = new List<GenericSymbol>();
                CurrentScope.BuildTacitGeneric(list);
                _TacitGeneric = list;
                return _TacitGeneric;
            }
        }

        internal override void BuildTacitGeneric(List<GenericSymbol> list)
        {
            if (CurrentScope != null)
            {
                CurrentScope.BuildTacitGeneric(list);
            }
            list.AddRange(Generics);
        }

        internal override OverLoadChain NameResolution(string name)
        {
            if (DisguiseScopeMode)
            {
                return CurrentScope.NameResolution(name);
            }
            if (ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            var n = CurrentScope.NameResolution(name);
            var i = InheritNameResolution(name);
            if (ChildSymbols.ContainsKey(name))
            {
                var s = ChildSymbols[name];
                n = new OverLoadChain(this, n, i, s);
            }
            else
            {
                n = new OverLoadChain(this, n, i);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        private IReadOnlyList<OverLoadChain> InheritNameResolution(string name)
        {
            var ret = new List<OverLoadChain>();
            DisguiseScopeMode = true;
            foreach (var v in Inherit)
            {
                var ol = v.NameResolution(name) as OverLoadChain;
                if (ol != null)
                {
                    ret.Add(ol);
                }
            }
            DisguiseScopeMode = false;
            return ret;
        }

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            throw new NotImplementedException();
        }

        internal virtual IEnumerable<OverLoadCallMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            throw new NotImplementedException();
        }

        internal virtual IEnumerable<TypeSymbol> EnumSubType()
        {
            throw new NotImplementedException();
        }

        internal static bool HasAnyErrorType(params TypeSymbol[] scope)
        {
            return HasAnyErrorType((IReadOnlyList<TypeSymbol>)scope);
        }

        internal static bool HasAnyErrorType(IReadOnlyList<TypeSymbol> scope)
        {
            foreach (var v in scope)
            {
                if(HasAnyErrorType(v))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool HasAnyErrorType(IReadOnlyList<GenericsInstance> gi)
        {
            foreach (var v in gi)
            {
                if (HasAnyErrorType(v.Generic, v.Type))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool HasAnyErrorType(TypeSymbol scope)
        {
            var cti = scope as ClassTemplateInstance;
            if(cti != null)
            {
                if (HasAnyErrorType(cti.Parameters) || HasAnyErrorType(cti.TacitGeneric))
                {
                    return true;
                }
            }
            else if (scope is VoidSymbol || scope is UnknownSymbol || scope is ErrorTypeSymbol)
            {
                return true;
            }
            return false;
        }
    }
}
