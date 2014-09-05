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
    public class BlockStructure : ExpressionStructure
    {
        public IReadOnlyList<CilStructure> Expressions { get; private set; }
        public bool IsInline { get; private set; }

        public BlockStructure(TypeStructure rt, IReadOnlyList<CilStructure> exps, bool isInline)
            :base(rt)
        {
            Expressions = exps;
            IsInline = isInline;
            AppendChild(exps);
        }

        internal override void BuildCode()
        {
            ChildBuildCode(this, IsInline);
        }

        internal void ChildBuildCode(CilStructure stru, bool isRet)
        {
            for (var i = 0; i < this.Count; ++i)
            {
                if (i == this.Count - 1 && isRet)
                {
                    this[i].BuildCode();
                }
                else
                {
                    PopBuildCode(this[i]);
                }
            }
        }

        public bool IsValueReturn
        {
            get 
            { 
                if(Expressions.Count == 0 || !IsInline)
                {
                    return false;
                }
                var exp = Expressions[Expressions.Count - 1] as ExpressionStructure;
                if(exp == null)
                {
                    return false;
                }
                return !exp.ResultType.IsVoid;
            }
        }
    }
}
