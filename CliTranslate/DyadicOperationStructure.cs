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
using AbstractSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class DyadicOperationStructure : ExpressionStructure
    {
        public ExpressionStructure Left { get; private set; }
        public ExpressionStructure Right { get; private set; }
        public BuilderStructure Call { get; private set; }
        public ExpressionStructure Next { get; private set; }

        public DyadicOperationStructure(TypeStructure rt, ExpressionStructure left, ExpressionStructure right, BuilderStructure call, ExpressionStructure next = null)
            :base(rt)
        {
            Left = left;
            Right = right;
            Call = call;
            Next = next;
            AppendChild(Left);
            if (Next == null)
            {
                AppendChild(Right);
            }
            else
            {
                AppendChild(Next);
            }
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            Left.BuildCode();
            Right.BuildCode();
            Call.BuildCall(cg);
            if(Next != null)
            {
                Next.BuildCode();
                cg.GenerateCode(OpCodes.And);
            }
        }
    }
}
