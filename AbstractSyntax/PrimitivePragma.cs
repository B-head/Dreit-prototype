using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AbstractSyntax
{
    public class PrimitivePragma : DeclateClass
    {
        public PrimitivePragmaType Type { get; private set; }

        public PrimitivePragma(PrimitivePragmaType type)
        {
            Type = type;
        }
    }

    public enum PrimitivePragmaType
    {
        Root,
        Integer32,
    }
}
