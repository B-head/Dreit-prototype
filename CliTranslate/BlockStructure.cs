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
                if(Expressions.Count == 0)
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
