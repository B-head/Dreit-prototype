using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

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
            var first = GetTextPosition(c);
            MoveNextToken(ref c);
            IdentifierAccess ident = IdentifierAccess(ref c);
            var last = ident.Position;
            Element explType = null;
            if (CheckToken(c, TokenType.Peir))
            {
                MoveNextToken(ref c);
                explType = MemberAccess(ref c);
                last = explType.Position;
            }
            return new DeclateVariant { Ident = ident, ExplicitType = explType, Position = SetTextLength(first, last) };
        }

        private DeclateRoutine DeclateRoutine(ref int c)
        {
            if (!CheckText(c, "rout", "routine"))
            {
                return null;
            }
            var p = GetTextPosition(c);
            MoveNextToken(ref c);
            IdentifierAccess ident = IdentifierAccess(ref c);
            TupleList attr = null;
            Element retType = null;
            if (CheckToken(c, TokenType.LeftParenthesis))
            {
                MoveNextToken(ref c);
                attr = ParseTuple(ref c, DeclateArgument);
                if (CheckToken(c, TokenType.RightParenthesis))
                {
                    MoveNextToken(ref c);
                }
            }
            else
            {
                attr = new TupleList();
            }
            if (CheckToken(c, TokenType.Peir))
            {
                MoveNextToken(ref c);
                retType = MemberAccess(ref c);
            }
            var block = Block(ref c);
            return new DeclateRoutine { Ident = ident, Argument = attr, ExplicitType = retType, Block = block, Position = SetTextLength(p, block.Position) };
        }

        private DeclateOperator DeclateOperator(ref int c)
        {
            if (!CheckText(c, "operator"))
            {
                return null;
            }
            var p = GetTextPosition(c);
            MoveNextToken(ref c);
            Token op = Read(c++);
            TupleList attr = null;
            Element retType = null;
            if (CheckToken(c, TokenType.LeftParenthesis))
            {
                MoveNextToken(ref c);
                attr = ParseTuple(ref c, DeclateArgument);
                if (CheckToken(c, TokenType.RightParenthesis))
                {
                    MoveNextToken(ref c);
                }
            }
            else
            {
                attr = new TupleList();
            }
            if (CheckToken(c, TokenType.Peir))
            {
                MoveNextToken(ref c);
                retType = MemberAccess(ref c);
            }
            var block = Block(ref c);
            return new DeclateOperator { Name = op.Text, Operator = op.Type, Argument = attr, ExplicitType = retType, Block = block, Position = SetTextLength(p, block.Position) };
        }

        private DeclateArgument DeclateArgument(ref int c)
        {
            IdentifierAccess ident = IdentifierAccess(ref c);
            if(ident == null)
            {
                return null;
            }
            var p = ident.Position;
            Element explType = null;
            if (CheckToken(c, TokenType.Peir))
            {
                MoveNextToken(ref c);
                explType = MemberAccess(ref c);
                p = SetTextLength(p, explType.Position);
            }
            return new DeclateArgument { Ident = ident, ExplicitType = explType, Position = p };
        }

        private DeclateClass DeclateClass(ref int c)
        {
            if (!CheckText(c, "class"))
            {
                return null;
            }
            var p = GetTextPosition(c);
            MoveNextToken(ref c);
            IdentifierAccess ident = IdentifierAccess(ref c);
            TupleList inherit = null;
            if (CheckToken(c, TokenType.Peir))
            {
                MoveNextToken(ref c);
                inherit = ParseTuple(ref c, MemberAccess);
            }
            else
            {
                inherit = new TupleList();
            }
            var block = Block(ref c);
            return new DeclateClass { Ident = ident, Inherit = inherit, Block = block, Position = SetTextLength(p, block.Position) };
        }
    }
}
