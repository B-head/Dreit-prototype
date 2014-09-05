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
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public abstract class MonadicExpression : Element
    {
        public TokenType Operator { get; private set; }
        public Element Exp { get; private set; }

        protected MonadicExpression(TextPosition tp, TokenType op, Element exp)
            :base(tp)
        {
            Operator = op;
            Exp = exp;
            AppendChild(Exp);
        }

        protected override string ElementInfo
        {
            get { return Enum.GetName(typeof(TokenType), Operator); }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (Exp == null)
            {
                cmm.CompileError("require-expression", this);
            }
        }
    }
}
