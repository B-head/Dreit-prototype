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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class RangeLiteral : Element
    {
        public Element Left { get; private set; }
        public Element Right { get; private set; }
        public bool IsLeftOpen { get; private set; }
        public bool IsRightOpen { get; private set; }

        public RangeLiteral(TextPosition tp, Element left, Element right, bool isLeftOpen = false, bool isRightOpen = false)
            :base(tp)
        {
            Left = left;
            Right = right;
            IsLeftOpen = isLeftOpen;
            IsRightOpen = isRightOpen;
            AppendChild(Left);
            AppendChild(Right);
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (Left == null)
            {
                cmm.CompileError("require-expression", this);
            }
            if (Right == null)
            {
                cmm.CompileError("require-expression", this);
            }
        }
    }
}
