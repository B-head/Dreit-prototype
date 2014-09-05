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
    public class IfStructure : ExpressionStructure
    {
        public ExpressionStructure Condition { get; private set; }
        public BlockStructure Then { get; private set; }
        public BlockStructure Else { get; private set; }
        public LabelStructure ElseLabel { get; private set; }
        public LabelStructure ExitLabel { get; private set; }

        public IfStructure(TypeStructure rt, ExpressionStructure cond, BlockStructure then, BlockStructure els)
            :base(rt)
        {
            Condition = cond;
            Then = then;
            Else = els;
            ElseLabel = new LabelStructure();
            ExitLabel = new LabelStructure();
            AppendChild(Condition);
            AppendChild(Then);
            AppendChild(Else);
            AppendChild(ElseLabel);
            AppendChild(ExitLabel);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            cg.BeginScope();
            Condition.BuildCode();
            cg.GenerateJump(OpCodes.Brfalse, ElseLabel);
            Then.BuildCode();
            cg.GenerateJump(OpCodes.Br, ExitLabel);
            cg.MarkLabel(ElseLabel);
            if (Else != null)
            {
                Else.BuildCode();
            }
            else if (Then.IsInline && !ResultType.IsVoid)
            {
                if (ResultType.IsValueType)
                {
                    var loc = new LocalStructure(ResultType, cg);
                    cg.GenerateLoad(loc);
                }
                else
                {
                    cg.GenerateCode(OpCodes.Ldnull);
                }
            }
            cg.MarkLabel(ExitLabel);
            cg.EndScope();
        }
    }
}
