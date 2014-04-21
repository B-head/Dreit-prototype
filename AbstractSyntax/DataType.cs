using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Pragma;

namespace AbstractSyntax
{
    public class DataType : Scope
    {
        public virtual PrimitivePragmaType GetPrimitiveType()
        {
            throw new NotSupportedException();
        }
    }

    enum TypeMatchResult
    {
        Unknown,
        PerfectMatch,
        NotCallable,
        MissMatchCount,
        MissMatchType,
    }
}
