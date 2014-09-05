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
using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    [Serializable]
    public class LoopStatement : Scope
    {
        public bool IsLater { get; private set; }
        public Element Condition { get; private set; }
        public Element Use { get; private set; }
        public Element By { get; private set; }
        public ProgramContext Block { get; private set; }

        public LoopStatement(TextPosition tp, bool isLater, Element cond, Element use, Element by, ProgramContext block)
            :base(tp)
        {
            IsLater = isLater;
            Condition = cond;
            Use = use;
            By = by;
            Block = block;
            AppendChild(Condition);
            AppendChild(Use);
            AppendChild(By);
            AppendChild(Block);
        }

        public bool IsDefinedCondition
        {
            get { return Condition != null; }
        }

        public bool IsDefinedOn
        {
            get { return Use != null; }
        }

        public bool IsDefinedBy
        {
            get { return By != null; }
        }

    }
}
