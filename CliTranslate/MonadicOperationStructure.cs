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

namespace CliTranslate
{
    [Serializable]
    public class MonadicOperationStructure : ExpressionStructure
    {
        public ExpressionStructure Expression { get; private set; }
        public BuilderStructure Call { get; private set; }

        public MonadicOperationStructure(TypeStructure rt, ExpressionStructure exp, BuilderStructure call)
            :base(rt)
        {
            Expression = exp;
            Call = call;
            AppendChild(Expression);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            Expression.BuildCode();
            Call.BuildCall(cg);
        }
    }
}
