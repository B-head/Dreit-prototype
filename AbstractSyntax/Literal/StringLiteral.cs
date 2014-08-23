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
    public class StringLiteral : Element
    {
        public IReadOnlyList<Element> Texts { get; private set; }
        public bool IsEfficient { get; private set; }
        private TypeSymbol _ReturnType;

        public StringLiteral(TextPosition tp, IReadOnlyList<Element> texts, bool isEff)
            :base(tp)
        {
            Texts = texts;
            IsEfficient = isEff;
            AppendChild(Texts);
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                if (_ReturnType == null)
                {
                    _ReturnType = CurrentScope.NameResolution("String").FindDataType().Type;
                }
                return _ReturnType;
            }
        }

        public override bool IsConstant
        {
            get { return true; }
        }

        public override dynamic GenerateConstantValue()
        {
            var build = new StringBuilder();
            foreach(var v in Texts)
            {
                build.Append(v.GenerateConstantValue());
            }
            return build.ToString();
        }
    }
}
