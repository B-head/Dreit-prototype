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

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class Postfix : MonadicExpression
    {
        public Postfix(TextPosition tp, TokenType op, Element exp)
            :base(tp, op, exp)
        {

        }

        public override TypeSymbol ReturnType
        {
            get
            {
                if(Operator == TokenType.Refer)
                {
                    return Root.ClassManager.Issue(Root.Refer, new TypeSymbol[] { Exp.OverLoad.FindDataType().Type }, new TypeSymbol[0]);
                }
                else if(Operator == TokenType.Typeof)
                {
                    return Root.ClassManager.Issue(Root.Typeof, new TypeSymbol[] { Exp.OverLoad.FindDataType().Type }, new TypeSymbol[0]);
                }
                else if(Operator == TokenType.Reject)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public override bool IsConstant
        {
            get { return true; }
        }
    }
}
