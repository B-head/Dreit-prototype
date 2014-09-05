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
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class IfStatement : Scope
    {
        public Element Condition { get; private set; }
        public ProgramContext Then { get; private set; }
        public ProgramContext Else { get; private set; }
        private TypeSymbol _ReturnType;

        public IfStatement(TextPosition tp, Element cond, ProgramContext then, ProgramContext els)
            :base(tp)
        {
            Condition = cond;
            Then = then;
            Else = els;
            AppendChild(Condition);
            AppendChild(Then);
            AppendChild(Else);
        }

        public bool IsDefinedElse
        {
            get { return Else != null; }
        }

        public override TypeSymbol ReturnType
        {
            get 
            {
                if(_ReturnType != null)
                {
                    return _ReturnType;
                }
                var a = BlockReturnType(Then);
                var b = BlockReturnType(Else);
                if (a == b || Else == null)
                {
                    _ReturnType = a;
                }
                else
                {
                    _ReturnType = Root.ErrorType;
                }
                return _ReturnType; 
            }
        }

        private TypeSymbol BlockReturnType(ProgramContext block)
        {
            if(block == null)
            {
                return Root.Void;
            }
            if(block.IsInline)
            {
                return block[0].ReturnType;
            }
            else
            {
                return Root.Void; //todo ifブロックで値を返すための構文が必要。
            }
        }
    }
}
