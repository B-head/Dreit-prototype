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
using AbstractSyntax.Expression;
using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class EnumSymbol : TypeSymbol
    {
        public ProgramContext Block { get; private set; }
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected TypeSymbol _BaseType;
        public IReadOnlyList<RoutineSymbol> Initializers { get; private set; }
        public IReadOnlyList<RoutineSymbol> AliasCalls { get; private set; }

        protected EnumSymbol(TextPosition tp, string name, ProgramContext block)
            : base(tp)
        {
            Name = name;
            Block = block;
            AppendChild(Block);
        }

        public EnumSymbol(string name, ProgramContext block, IReadOnlyList<AttributeSymbol> attr, TypeSymbol bt)
        {
            Name = name;
            Block = block;
            _Attribute = attr;
            _BaseType = bt;
            AppendChild(Block);
        }

        public override IReadOnlyList<GenericSymbol> Generics
        {
            get { return new List<GenericSymbol>(); }
        }

        public override IReadOnlyList<TypeSymbol> Inherit
        {
            get { return new TypeSymbol[] { BaseType }; }
        }

        public virtual TypeSymbol BaseType
        {
            get { return _BaseType; }
        }

        internal override void Prepare()
        {
            InitInitializers();
            InitAliasCalls();
        }

        private void InitInitializers()
        {
            var i = new List<RoutineSymbol>();
            var def = new DefaultSymbol(RoutineSymbol.ConstructorIdentifier, this);
            Block.Append(def);
            i.Add(def);
            Initializers = i;
        }

        private void InitAliasCalls()
        {
            var i = new List<RoutineSymbol>();
            var g = new PropertySymbol(RoutineSymbol.AliasCallIdentifier, this, false);
            Block.Append(g);
            i.Add(g);
            var s = new PropertySymbol(RoutineSymbol.AliasCallIdentifier, this, true);
            Block.Append(s);
            i.Add(s);
            AliasCalls = i;
        }

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            var newinst = GenericsInstance.MakeGenericInstance(Generics, pars);
            var newpars = new List<TypeSymbol>();
            foreach (var a in Initializers)
            {
                foreach (var b in a.GetTypeMatch(newinst, newpars, args))
                {
                    yield return b;
                }
            }
            foreach (var a in Root.ConvManager.GetAllInitializer(this))
            {
                foreach (var b in a.GetTypeMatch(newinst, newpars, args))
                {
                    yield return b;
                }
            }
        }

        internal override IEnumerable<OverLoadCallMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            foreach (var a in AliasCalls)
            {
                foreach (var b in a.GetTypeMatch(inst, pars, args))
                {
                    yield return b;
                }
            }
        }

        internal override IEnumerable<TypeSymbol> EnumSubType()
        {
            yield return this;
            foreach (var a in Inherit)
            {
                foreach (var b in a.EnumSubType())
                {
                    yield return b;
                }
            }
        }
    }
}
