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
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class PropertySymbol : RoutineSymbol
    {
        public TypeSymbol Type { get; private set; }
        public bool IsSet { get; private set; }

        public PropertySymbol(string name, TypeSymbol type, bool isSet)
            : base(RoutineType.Routine, TokenType.Unknoun)
        {
            Name = name;
            Type = type;
            IsSet = isSet;
            _Attribute = null;
            _Arguments = null;
        }

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get
            {
                if (_Attribute == null)
                {
                    _Attribute = Type.Attribute;
                }
                return _Attribute;
            }
        }

        public override IReadOnlyList<ArgumentSymbol> Arguments
        {
            get
            {
                if (_Arguments == null)
                {
                    if (IsSet)
                    {
                        _Arguments = ArgumentSymbol.MakeParameters(Type);
                    }
                    else
                    {
                        _Arguments = new ArgumentSymbol[] { };
                    }
                }
                return _Arguments;
            }
        }

        public override TypeSymbol CallReturnType
        {
            get { return Type; }
        }
    }
}
