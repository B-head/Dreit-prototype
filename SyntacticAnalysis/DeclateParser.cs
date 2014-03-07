using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using Common;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private DeclateVariant DeclareVariant(ref int c)
        {
            if (!CheckText(c, "var", "variant"))
            {
                return null;
            }
            SkipSpaser(++c);
            Identifier ident = Identifier(ref c);
            Element explType = null;
            if (CheckToken(c, TokenType.Peir))
            {
                SkipSpaser(++c);
                explType = MemberAccess(ref c);
            }
            return new DeclateVariant { Ident = ident, ExplicitVariantType = explType, Position = ident.Position };
        }

        private DeclateRoutine DeclateRoutine(ref int c)
        {
            if (!CheckText(c, "rout", "routine"))
            {
                return null;
            }
            SkipSpaser(++c);
            Identifier ident = Identifier(ref c);
            TupleList<DeclateArgument> attr = null;
            Element retType = null;
            Element block = null;
            if (CheckToken(c, TokenType.LeftParenthesis))
            {
                SkipSpaser(++c);
                attr = ParseTuple(ref c, DeclateArgument);
                if (CheckToken(c, TokenType.RightParenthesis))
                {
                    SkipSpaser(++c);
                }
            }
            if (CheckToken(c, TokenType.Peir))
            {
                SkipSpaser(++c);
                retType = MemberAccess(ref c);
            }
            block = Block(ref c);
            return new DeclateRoutine { Ident = ident, Argument = attr, ExplicitResultType = retType, Block = block, Position = ident.Position };
        }

        private DeclateArgument DeclateArgument(ref int c)
        {
            Identifier ident = Identifier(ref c);
            if(ident == null)
            {
                return null;
            }
            Element explType = null;
            if (CheckToken(c, TokenType.Peir))
            {
                SkipSpaser(++c);
                explType = MemberAccess(ref c);
            }
            return new DeclateArgument { Ident = ident, ExplicitArgumentType = explType, Position = ident.Position };
        }

        private DeclateClass DeclateClass(ref int c)
        {
            if (!CheckText(c, "class"))
            {
                return null;
            }
            SkipSpaser(++c);
            Identifier ident = Identifier(ref c);
            Element block = Block(ref c);
            return new DeclateClass { Ident = ident, Block = block, Position = ident.Position };
        }
    }
}
