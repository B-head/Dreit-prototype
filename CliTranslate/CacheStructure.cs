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

        public CacheStructure(TypeStructure rt, ExpressionStructure exp)
            :base(rt)
        {
            Expression = exp;
            Cache = new LocalStructure(Expression.ResultType);
            AppendChild(Expression);
            AppendChild(Cache);
        }
    }
}
