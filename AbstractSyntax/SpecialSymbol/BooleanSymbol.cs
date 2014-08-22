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
    public class BooleanSymbol : VariantSymbol
    {
        public bool Value { get; private set; }

        public BooleanSymbol(bool value)
            : base(VariantType.Const)
        {
            Value = value;
            if(value)
            {
                Name = "true";
            }
            else
            {
                Name = "false";
            }
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                if (_DataType != null)
                {
                    return _DataType;
                }
                _DataType = CurrentScope.NameResolution("Boolean").FindDataType().Type;
                return _DataType;
            }
        }
    }
}
