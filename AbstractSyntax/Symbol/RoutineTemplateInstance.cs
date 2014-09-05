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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class RoutineTemplateInstance : RoutineSymbol
    {
        public RoutineSymbol Routine { get; private set; }
        public IReadOnlyList<TypeSymbol> Parameters { get; private set; }
        public IReadOnlyList<TypeSymbol> TacitParameters { get; private set; }

        public RoutineTemplateInstance(RoutineSymbol routine, IReadOnlyList<TypeSymbol> parameters, IReadOnlyList<TypeSymbol> tacitParameters)
            :base(RoutineType.Unknown, TokenType.Unknoun)
        {
            Routine = routine;
            Parameters = parameters;
            TacitParameters = tacitParameters;
        }

        protected override string ElementInfo
        {
            get
            {
                if (Parameters.Count == 0)
                {
                    return string.Format("{0}", Routine.Name);
                }
                else
                {
                    return string.Format("{0}!({1})", Routine.Name, Parameters.ToNames());
                }
            }
        }

        internal IReadOnlyList<GenericsInstance> GetGenericInstance()
        {
            var p = GenericsInstance.MakeGenericInstance(Routine.Generics, Parameters);
            var tp = GenericsInstance.MakeGenericInstance(Routine.TacitGeneric, TacitParameters);
            return p.Concat(tp).ToList();
        }

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get { return Routine.Attribute; }
        }

        public override IReadOnlyList<GenericSymbol> Generics
        {
            get { return Routine.Generics; }
        }

        public override IReadOnlyList<ArgumentSymbol> Arguments
        {
            get { return GenericsInstance.MakeArgumentTemplateInstanceList(Root, GetGenericInstance(), Routine.Arguments); }
        }

        public override TypeSymbol CallReturnType
        {
            get 
            {
                if(_CallReturnType == null)
                {
                    _CallReturnType = GenericsInstance.MakeClassTemplateInstance(Root, GetGenericInstance(), Routine.CallReturnType);
                }
                return _CallReturnType; 
            }
        }

        public TypeSymbol DeclaringInstance
        {
            get { return GenericsInstance.MakeClassTemplateInstance(Root, GetGenericInstance(), Routine.GetParent<TypeSymbol>()); }
        }

        public override bool IsConstructor
        {
            get { return Routine.IsConstructor; }
        }

        public override bool IsDestructor
        {
            get { return Routine.IsDestructor; }
        }

        public override bool IsAliasCall
        {
            get { return Routine.IsAliasCall; }
        }

        public override TypeSymbol DeclaringType
        {
            get { return Routine.DeclaringType; }
        }

        public override bool IsInstanceMember
        {
            get { return Routine.IsInstanceMember; }
        }

        public override bool IsStaticMember
        {
            get { return Routine.IsStaticMember; }
        }
    }
}
