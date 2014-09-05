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

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class ArrayLiteral : Element
    {
        public IReadOnlyList<Element> Values { get; private set; }
        private TypeSymbol _BaseType;
        private TypeSymbol _ReturnType;

        public ArrayLiteral(TextPosition tp, IReadOnlyList<Element> values)
            :base(tp)
        {
            Values = values;
            AppendChild(Values);
        }

        public TypeSymbol BaseType
        {
            get
            {
                if(_BaseType != null)
                {
                    return _BaseType;
                }
                if(Values.Count == 0)
                {
                    _BaseType = CurrentScope.NameResolution("Object").FindDataType().Type;
                }
                else
                {
                    _BaseType = Values[0].ReturnType;
                }
                return _BaseType;
            }
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                if (_ReturnType != null)
                {
                    return _ReturnType;
                }
                var list = CurrentScope.NameResolution("List").FindDataType().Type;
                _ReturnType = Root.ClassManager.Issue(list, new TypeSymbol[] { BaseType }, new TypeSymbol[0]);
                return _ReturnType;
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            foreach (var v in Values)
            {
                if (BaseType != v.ReturnType)
                {
                    cmm.CompileError("disagree-array-type", this);
                }
            }
        }
    }
}
