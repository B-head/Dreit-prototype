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
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class LogicalStructure : ExpressionStructure
    {
        public ExpressionStructure Left { get; private set; }
        public ExpressionStructure Right { get; private set; }
        public bool IsOr { get; private set;}
        public LabelStructure Exit { get; private set; }

        public LogicalStructure(TypeStructure rt, ExpressionStructure left, ExpressionStructure right, bool isOr)
            :base(rt)
        {
            Left = left;
            Right = right;
            IsOr = isOr;
            Exit = new LabelStructure();
            AppendChild(Left);
            AppendChild(Right);
            AppendChild(Exit);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            if (IsOr)
            {
                Left.BuildCode();
                cg.GenerateCode(OpCodes.Dup);
                cg.GenerateJump(OpCodes.Brtrue, Exit);
                cg.GenerateCode(OpCodes.Pop);
                Right.BuildCode();
                cg.MarkLabel(Exit);
            }
            else
            {
                Left.BuildCode();
                cg.GenerateCode(OpCodes.Dup);
                cg.GenerateJump(OpCodes.Brfalse, Exit);
                cg.GenerateCode(OpCodes.Pop);
                Right.BuildCode();
                cg.MarkLabel(Exit);
            }
        }
    }
}
