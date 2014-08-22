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
    public class AttributeSymbol : Scope
    {
        public AttributeType AttributeType { get; private set; }
        public AttributeTargets ValidOn { get; private set; }
        public bool IsAllowMultiple { get; private set; }
        public bool IsInheritable { get; private set; }

        public AttributeSymbol(AttributeType type, string name = null)
        {
            Name = name;
            AttributeType = type;
            ValidOn = AttributeTargets.All;
        }
    }

    //todo 単純なフラグ管理で属性を扱うようにリファクタリングする。
    public enum AttributeType
    {
        Unknown,
        Custom,
        Contravariant,
        Covariant,
        ConstructorConstraint,
        ValueConstraint,
        ReferenceConstraint,
        Variadic,
        Optional,
        GlobalScope,
        Abstract,
        Virtual,
        Final,
        Static,
        Public,
        Internal,
        Protected,
        Private,
    }
}
