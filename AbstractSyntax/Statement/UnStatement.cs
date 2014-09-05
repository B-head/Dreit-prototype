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
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class UnStatement : Element
    {
        public Element Exp { get; private set; }
        private RoutineSymbol _CallRoutine;

        public UnStatement(TextPosition tp, Element exp)
            :base(tp)
        {
            Exp = exp;
            AppendChild(Exp);
        }

        public RoutineSymbol CallRoutine
        {
            get
            {
                if (_CallRoutine == null)
                {
                    _CallRoutine = Root.OpManager.FindMonadic(TokenType.Not, Exp.ReturnType);
                }
                return _CallRoutine;
            }
        }

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
        }

        public override bool IsConstant
        {
            get { return Exp.IsConstant && CallRoutine.IsFunction; }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (CallRoutine is ErrorRoutineSymbol)
            {
                cmm.CompileError("undefined-monadic-operator", this);
            }
        }
    }
}
