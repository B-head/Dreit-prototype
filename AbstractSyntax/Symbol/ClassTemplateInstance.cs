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
    public class ClassTemplateInstance : ClassSymbol
    {
        public TypeSymbol Type { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }
        public IReadOnlyList<TypeSymbol> TacitParameters { get; private set; }

        public ClassTemplateInstance(TypeSymbol type, IReadOnlyList<TypeSymbol> parameters, IReadOnlyList<TypeSymbol> tacitParameters)
            :base(ClassType.Unknown)
        {
            //if (type.Generics.Count != parameters.Count || type.TacitGeneric.Count != tacitParameters.Count)
            //{
            //    throw new ArgumentException("parameter count");
            //}
            Type = type;
            Parameters = parameters;
            TacitParameters = tacitParameters;
            InitializeChildSymbols();
        }

        private void InitializeChildSymbols()
        {
            foreach (var v in Type.ChildSymbols)
            {
                ChildSymbols.Add(v.Key, v.Value.Clone(this));
            }
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

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            inst = GetGenericInstance();
            foreach (var v in Type.GetTypeMatch(inst, pars, args))
            {
                yield return v;
            }
        }

        internal override IEnumerable<OverLoadCallMatch> GetInstanceMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            if (Type == Root.Typeof)
            {
                inst = GetGenericInstance();
                foreach (var v in Parameters[0].GetTypeMatch(inst, pars, args))
                {
                    yield return v;
                }
            }
            else
            {
                inst = GetGenericInstance();
                foreach (var v in Type.GetInstanceMatch(inst, pars, args))
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

        internal IReadOnlyList<GenericsInstance> GetGenericInstance()
        {
            var p = GenericsInstance.MakeGenericInstance(Type.Generics, Parameters);
            var tp = GenericsInstance.MakeGenericInstance(Type.TacitGeneric, TacitParameters);
            return p.Concat(tp).ToList();
        }

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get { return Type.Attribute; }
        }

        public override IReadOnlyList<GenericSymbol> Generics
        {
            get { return new List<GenericSymbol>(); }
        }

        public override IReadOnlyList<TypeSymbol> Inherit
        {
            get { return GenericsInstance.MakeClassTemplateInstanceList(Root, GetGenericInstance(), Type.Inherit); }
        }

        public override TypeSymbol DeclaringType
        {
            get { return Type.DeclaringType; }
        }

        public override bool IsInstanceMember
        {
            get { return Type.IsInstanceMember; }
        }

        public override bool IsStaticMember
        {
            get { return Type.IsStaticMember; }
        }
    }
}
