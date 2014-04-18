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
        private DirectiveList DirectiveList(ref int c, bool root = false)
        {
            DirectiveList result = new DirectiveList();
            while (IsReadable(c))
            {
                if (CheckToken(c, TokenType.EndExpression))
                {
                    MoveNextToken(ref c);
                    continue;
                }
                Element temp = Directive(ref c);
                if (temp == null)
                {
                    if (!root && CheckToken(c, TokenType.RightBrace))
                    {
                        break;
                    }
                    AddError(c);
                    MoveNextToken(ref c);
                    continue;
                }
                result.Append(temp);
            }
            return result;
        }

        private DirectiveList Block(ref int c)
        {
            DirectiveList result = null;
            var first = GetTextPosition(c);
            if (CheckToken(c, TokenType.Separator))
            {
                MoveNextToken(ref c);
                result = new DirectiveList { IsInline = true };
                var d = Directive(ref c);
                result.Append(d);
                result.Position = SetTextLength(first, d.Position);
            }
            else if (CheckToken(c, TokenType.LeftBrace))
            {
                TextPosition last;
                MoveNextToken(ref c);
                result = DirectiveList(ref c);
                if (CheckToken(c, TokenType.RightBrace))
                {
                    last = GetTextPosition(c);
                    MoveNextToken(ref c);
                }
                else
                {
                    last = result[result.Count - 1].Position;
                }
                result.Position = SetTextLength(first, last);
            }
            return result;
        }

        private Element Directive(ref int c)
        {
            Element temp = CoalesceParser(ref c, Echo, Alias, Return, Expression);
            if (CheckToken(c, TokenType.EndExpression))
            {
                MoveNextToken(ref c);
            }
            return temp;
        }

        private EchoDirective Echo(ref int c)
        {
            if (!CheckText(c, "echo"))
            {
                return null;
            }
            var p = GetTextPosition(c);
            MoveNextToken(ref c);
            Element exp = Expression(ref c);
            return new EchoDirective { Exp = exp, Position = SetTextLength(p, exp.Position) };
        }

        private AliasDirective Alias(ref int c)
        {
            if (!CheckText(c, "alias"))
            {
                return null;
            }
            var p = GetTextPosition(c);
            MoveNextToken(ref c);
            var from = IdentifierAccess(ref c);
            var to = IdentifierAccess(ref c);
            return new AliasDirective { From = from, To = to, Position = SetTextLength(p, to.Position) };
        }

        private ReturnDirective Return(ref int c)
        {
            if (!CheckText(c, "return"))
            {
                return null;
            }
            var p = GetTextPosition(c);
            MoveNextToken(ref c);
            Element exp = Expression(ref c);
            return new ReturnDirective { Exp = exp, Position = SetTextLength(p, exp.Position) };
        }
    }
}
