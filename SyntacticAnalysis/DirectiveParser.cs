using AbstractSyntax;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static DirectiveList DirectiveList(ChainParser cp, bool root = false)
        {
            return cp.Begin<DirectiveList>()
                .Skip(TokenType.EndExpression, TokenType.LineTerminator)
                .End();
            /*
            DirectiveList result = new DirectiveList();
            while (c.IsReadable(i))
            {
                if (c.CheckToken(i, TokenType.EndExpression))
                {
                    c.MoveNextToken(ref i);
                    continue;
                }
                Element temp = Directive(c, ref i);
                if (temp == null)
                {
                    if (!root && c.CheckToken(i, TokenType.RightBrace))
                    {
                        break;
                    }
                    c.AddError(i);
                    c.MoveNextToken(ref i);
                    continue;
                }
                result.Append(temp);
            }
            return result;
             */
        }

        private static DirectiveList Block(TokenCollection c, ref int i)
        {
            DirectiveList result = null;
            var first = c.GetTextPosition(i);
            if (c.CheckToken(i, TokenType.Separator))
            {
                c.MoveNextToken(ref i);
                result = new DirectiveList { IsInline = true };
                var d = Directive(c, ref i);
                result.Append(d);
                result.Position = first.AlterLength((TextPosition?)d);
            }
            else if (c.CheckToken(i, TokenType.LeftBrace))
            {
                TextPosition last;
                c.MoveNextToken(ref i);
                //result = DirectiveList(c, ref i);
                if (c.CheckToken(i, TokenType.RightBrace))
                {
                    last = c.GetTextPosition(i);
                    c.MoveNextToken(ref i);
                }
                else
                {
                    last = result[result.Count - 1].Position;
                }
                result.Position = first.AlterLength(last);
            }
            else
            {
                result = new DirectiveList { Position = c.GetTextPosition(i) };
            }
            return result;
        }

        private static Element Directive(TokenCollection c, ref int i)
        {
            Element temp = CoalesceParser(c, ref i, Echo, Alias, Return, Expression);
            if (c.CheckToken(i, TokenType.EndExpression))
            {
                c.MoveNextToken(ref i);
            }
            return temp;
        }

        private static EchoDirective Echo(TokenCollection c, ref int i)
        {
            if (!c.CheckText(i, "echo"))
            {
                return null;
            }
            var p = c.GetTextPosition(i);
            c.MoveNextToken(ref i);
            Element exp = Expression(c, ref i);
            return new EchoDirective { Exp = exp, Position = p.AlterLength((TextPosition?)exp) };
        }

        private static AliasDirective Alias(TokenCollection c, ref int i)
        {
            if (!c.CheckText(i, "alias"))
            {
                return null;
            }
            var p = c.GetTextPosition(i);
            c.MoveNextToken(ref i);
            var from = IdentifierAccess(c, ref i);
            var to = IdentifierAccess(c, ref i);
            return new AliasDirective { From = from, To = to, Position = p.AlterLength((TextPosition?)to) };
        }

        private static ReturnDirective Return(TokenCollection c, ref int i)
        {
            if (!c.CheckText(i, "return"))
            {
                return null;
            }
            var p = c.GetTextPosition(i);
            c.MoveNextToken(ref i);
            Element exp = Expression(c, ref i);
            return new ReturnDirective { Exp = exp, Position = p.AlterLength((TextPosition?)exp) };
        }
    }
}
