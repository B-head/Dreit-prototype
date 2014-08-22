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

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class AttributeScope : Element
    {
        private List<AttributeSymbol> _Attribute;

        public AttributeScope(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        public IReadOnlyList<AttributeSymbol> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                _Attribute = new List<AttributeSymbol>();
                foreach (var v in this)
                {
                    _Attribute.Add(v.OverLoad.FindAttribute());
                }
                return _Attribute;
            }
        }
    }
}
