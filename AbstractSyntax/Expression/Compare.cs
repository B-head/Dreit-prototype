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

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Compare : DyadicExpression
    {
        private RoutineSymbol _CallRoutine;

        public Compare(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, op, left, right)
        {

        }

        public RoutineSymbol CallRoutine
        {
            get
            {
                if (_CallRoutine == null)
                {
                    _CallRoutine = Root.OpManager.FindDyadic(Operator, Left.ReturnType, VirtualRight.ReturnType);
                }
                return _CallRoutine;
            }
        }

        public override bool IsConstant
        {
            get { return Left.IsConstant && Right.IsConstant && CallRoutine.IsFunction; }
        }

        public override TypeSymbol ReturnType
        {
            get { return CallRoutine.CallReturnType; }
        }

        public bool IsLeftConnection
        {
            get { return Parent is Compare; }
        }

        public bool IsRightConnection
        {
            get { return Right is Compare; }
        }

        public Element VirtualRight
        {
            get
            {
                if (IsRightConnection)
                {
                    var c = (Compare)Right;
                    return c.Left;
                }
                else
                {
                    return Right;
                }
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (CallRoutine is ErrorRoutineSymbol && !TypeSymbol.HasAnyErrorType(Left.ReturnType, Right.ReturnType))
            {
                cmm.CompileError("impossible-calculate", this);
            }
        }
    }
}
