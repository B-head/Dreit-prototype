using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Statement
{
    public enum ScopeGuardType
    {
        Unknown,
        Exit,
        Success,
        Failure,
    }

    [Serializable]
    public class ScopeGuardStatement : Scope
    {
        public ScopeGuardType ScopeGuardType { get; private set; }
        public ProgramContext Block { get; private set; }

        public ScopeGuardStatement(TextPosition tp, ScopeGuardType type, ProgramContext block)
            :base(tp)
        {
            ScopeGuardType = type;
            Block = block;
            AppendChild(Block);
        }
    }
}
