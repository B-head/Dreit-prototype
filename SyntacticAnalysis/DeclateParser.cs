using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static DeclateVariant DeclareVariant(TokenCollection c, ref int i)
        {
            if (!c.CheckText(i, "var"))
            {
                return null;
            }
            var first = c.GetTextPosition(i);
            c.MoveNextToken(ref i);
            IdentifierAccess ident = IdentifierAccess(c, ref i);
            var last = ident.Position;
            Element explType = null;
            if (c.CheckToken(i, TokenType.Peir))
            {
                c.MoveNextToken(ref i);
                explType = MemberAccess(c, ref i);
                last = explType.Position;
            }
            return new DeclateVariant { Ident = ident, ExplicitType = explType, Position = first.AlterLength(last) };
        }

        private static DeclateRoutine DeclateRoutine(TokenCollection c, ref int i)
        {
            if (!c.CheckText(i, "rout", "routine"))
            {
                return null;
            }
            var p = c.GetTextPosition(i);
            c.MoveNextToken(ref i);
            IdentifierAccess ident = IdentifierAccess(c, ref i);
            TupleList attr = null;
            Element retType = null;
            if (c.CheckToken(i, TokenType.LeftParenthesis))
            {
                c.MoveNextToken(ref i);
                attr = ParseTuple(c, ref i, DeclateArgument);
                if (c.CheckToken(i, TokenType.RightParenthesis))
                {
                    c.MoveNextToken(ref i);
                }
            }
            else
            {
                attr = new TupleList();
            }
            if (c.CheckToken(i, TokenType.Peir))
            {
                c.MoveNextToken(ref i);
                retType = MemberAccess(c, ref i);
            }
            var block = Block(c, ref i);
            return new DeclateRoutine { Ident = ident, Argument = attr, ExplicitType = retType, Block = block, Position = p.AlterLength((TextPosition?)block) };
        }

        private static DeclateOperator DeclateOperator(TokenCollection c, ref int i)
        {
            if (!c.CheckText(i, "operator"))
            {
                return null;
            }
            var p = c.GetTextPosition(i);
            c.MoveNextToken(ref i);
            Token op = c.Read(i++);
            TupleList attr = null;
            Element retType = null;
            if (c.CheckToken(i, TokenType.LeftParenthesis))
            {
                c.MoveNextToken(ref i);
                attr = ParseTuple(c, ref i, DeclateArgument);
                if (c.CheckToken(i, TokenType.RightParenthesis))
                {
                    c.MoveNextToken(ref i);
                }
            }
            else
            {
                attr = new TupleList();
            }
            if (c.CheckToken(i, TokenType.Peir))
            {
                c.MoveNextToken(ref i);
                retType = MemberAccess(c, ref i);
            }
            var block = Block(c, ref i);
            return new DeclateOperator { Name = op.Text, Operator = op.Type, Argument = attr, ExplicitType = retType, Block = block, Position = p.AlterLength((TextPosition?)block) };
        }

        private static DeclateArgument DeclateArgument(TokenCollection c, ref int i)
        {
            IdentifierAccess ident = IdentifierAccess(c, ref i);
            if(ident == null)
            {
                return null;
            }
            var p = ident.Position;
            Element explType = null;
            if (c.CheckToken(i, TokenType.Peir))
            {
                c.MoveNextToken(ref i);
                explType = MemberAccess(c, ref i);
                p = p.AlterLength((TextPosition?)explType);
            }
            return new DeclateArgument { Ident = ident, ExplicitType = explType, Position = p };
        }

        private static DeclateClass DeclateClass(TokenCollection c, ref int i)
        {
            if (!c.CheckText(i, "class"))
            {
                return null;
            }
            var p = c.GetTextPosition(i);
            c.MoveNextToken(ref i);
            IdentifierAccess ident = IdentifierAccess(c, ref i);
            TupleList inherit = null;
            if (c.CheckToken(i, TokenType.Peir))
            {
                c.MoveNextToken(ref i);
                inherit = ParseTuple(c, ref i, MemberAccess);
            }
            else
            {
                inherit = new TupleList();
            }
            var block = Block(c, ref i);
            return new DeclateClass { Ident = ident, Inherit = inherit, Block = block, Position = p.AlterLength((TextPosition?)block) };
        }
    }
}
