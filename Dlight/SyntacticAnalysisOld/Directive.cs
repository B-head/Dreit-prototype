using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.SyntacticAnalysisOld
{
    partial class Parser
    {
        private Syntax Directive(ref int c)
        {
            Syntax s = Spacer(ref c);
            if(s.Child.Count > 0)
            {
                return s;
            }
            return CoalesceParser
                (
                ref c,
                SelectToken(SyntaxType.EndDirective),
                Import,
                Using,
                Expression,
                SkipError
                );
        }

        private Syntax Import(ref int c)
        {
            return SequenceParser(SyntaxType.Import, ref c, null, CheckText("import", "include"), Spacer, ArgumentList);
        }

        private Syntax Using(ref int c)
        {
            return SequenceParser(SyntaxType.Using, ref c, null, CheckText("using"), Spacer, ArgumentList);
        }

        private Syntax Alias(ref int c)
        {
            return SequenceParser(SyntaxType.Alias, ref c, null, CheckText("alias"), Spacer, ParentAccess, Spacer, ParentAccess);
        }

        private Syntax WildAttribute(ref int c)
        {
            return SequenceParser(SyntaxType.WildAttribute, ref c, null, SelectToken(SyntaxType.Wild), Spacer, ParentAccess);
        }
    }
}
