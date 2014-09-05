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
    public class LoopStructure : ExpressionStructure
    {
        public ExpressionStructure Condition { get; private set; }
        public ExpressionStructure Use { get; private set; }
        public ExpressionStructure By { get; private set; }
        public BlockStructure Block { get; private set; }
        public LabelStructure BreakLabel { get; private set; }
        public LabelStructure ContinueLabel { get; private set; }
        public LabelStructure PlungeLabel { get; private set; }

        public LoopStructure(TypeStructure rt)
            :base(rt)
        {
            BreakLabel = new LabelStructure();
            ContinueLabel = new LabelStructure();
            PlungeLabel = new LabelStructure();
            AppendChild(BreakLabel);
            AppendChild(ContinueLabel);
            AppendChild(PlungeLabel);
        }

        public void Initialize(ExpressionStructure cond, ExpressionStructure use, ExpressionStructure by, BlockStructure block)
        {
            Condition = cond;
            Use = use;
            By = by;
            Block = block;
            AppendChild(Condition);
            AppendChild(Use);
            AppendChild(By);
            AppendChild(Block);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.BeginScope();
            if (Use != null)
            {
                PopBuildCode(Use);
            }
            cg.GenerateJump(OpCodes.Br, PlungeLabel);
            cg.MarkLabel(ContinueLabel);
            if (By != null)
            {
                PopBuildCode(By);
            }
            cg.MarkLabel(PlungeLabel);
            if (Condition != null)
            {
                Condition.BuildCode();
                cg.GenerateJump(OpCodes.Brfalse, BreakLabel);
            }
            Block.BuildCode();
            if(Block.IsValueReturn)
            {
                cg.GenerateCode(OpCodes.Pop);
            }
            cg.GenerateJump(OpCodes.Br, ContinueLabel);
            cg.MarkLabel(BreakLabel);
            cg.EndScope();
        }
    }
}
