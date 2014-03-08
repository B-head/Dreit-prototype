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
            if (!CheckText(c, "var"))
            {
                return null;
            }
            SkipSpaser(++c);
            IdentifierAccess ident = Identifier(ref c);
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
            IdentifierAccess ident = Identifier(ref c);
            TupleList attr = null;
            Element retType = null;
            if (CheckToken(c, TokenType.LeftParenthesis))
            {
                SkipSpaser(++c);
                attr = ParseTuple(ref c, DeclateArgument);
                if (CheckToken(c, TokenType.RightParenthesis))
                {
                    SkipSpaser(++c);
                }
            }
            else
            {
                attr = new TupleList();
            }
            if (CheckToken(c, TokenType.Peir))
            {
                SkipSpaser(++c);
                retType = MemberAccess(ref c);
            }
            var block = Block(ref c);
            return new DeclateRoutine { Ident = ident, Argument = attr, ExplicitResultType = retType, Block = block, Position = ident.Position };
        }

        private DeclateOperator DeclateOperator(ref int c)
        {
            if (!CheckText(c, "operator"))
            {
                return null;
            }
            SkipSpaser(++c);
            Token op = Read(c++);
            TupleList attr = null;
            Element retType = null;
            if (CheckToken(c, TokenType.LeftParenthesis))
            {
                SkipSpaser(++c);
                attr = ParseTuple(ref c, DeclateArgument);
                if (CheckToken(c, TokenType.RightParenthesis))
                {
                    SkipSpaser(++c);
                }
            }
            else
            {
                attr = new TupleList();
            }
            if (CheckToken(c, TokenType.Peir))
            {
                SkipSpaser(++c);
                retType = MemberAccess(ref c);
            }
            var block = Block(ref c);
            return new DeclateOperator { Name = op.Text, Operator = op.Type, Argument = attr, ExplicitResultType = retType, Block = block, Position = op.Position };
        }

        private DeclateArgument DeclateArgument(ref int c)
        {
            IdentifierAccess ident = Identifier(ref c);
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
            IdentifierAccess ident = Identifier(ref c);
            TupleList inherit = null;
            if (CheckToken(c, TokenType.Peir))
            {
                SkipSpaser(++c);
                inherit = ParseTuple(ref c, MemberAccess);
            }
            else
            {
                inherit = new TupleList();
            }
            var block = Block(ref c);
            return new DeclateClass { Ident = ident, Inherit = inherit, Block = block, Position = ident.Position };
        }
    }
}
