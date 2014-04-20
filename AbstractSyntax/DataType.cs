using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class DataType : Scope
    {
        //いつか使うつもり。
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
