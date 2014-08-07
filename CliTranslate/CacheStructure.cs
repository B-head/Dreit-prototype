using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class CacheStructure : ExpressionStructure
    {
        public ExpressionStructure Expression { get; private set; }
        public LocalStructure Cache { get; private set; }
        private bool IsCache;

        public CacheStructure(TypeStructure rt, ExpressionStructure exp)
            :base(rt)
        {
            Expression = exp;
            Cache = new LocalStructure(Expression.ResultType);
            AppendChild(Expression);
            AppendChild(Cache);
        }

        internal override void BuildCode()
        {
            var cg = CurrentContainer.GainGenerator();
            if(!IsCache)
            {
                IsCache = true;
                Expression.BuildCode();
                cg.GenerateStore(Cache);
            }
            cg.GenerateLoad(Cache);
        }
    }
}
