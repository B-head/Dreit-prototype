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
                    SkipLineTerminator(++c);
                    continue;
                }
                Element temp = Directive(ref c);
                if (temp == null)
                {
                    if (!root && CheckToken(c, TokenType.RightBrace))
                    {
                        break;
                    }
                    SkipError(c);
                    continue;
                }
                result.Append(temp);
            }
            return result;
        }

        private DirectiveList Block(ref int c)
        {
            DirectiveList result = null;
            if (CheckToken(c, TokenType.Separator))
            {
                SkipLineTerminator(++c);
                result = new DirectiveList { IsInline = true };
                result.Append(Directive(ref c));
            }
            else if (CheckToken(c, TokenType.LeftBrace))
            {
                SkipLineTerminator(++c);
                result = DirectiveList(ref c);
                if (CheckToken(c, TokenType.RightBrace))
                {
                    SkipLineTerminator(++c);
                }
            }
            return result;
        }

        private Element Directive(ref int c)
        {
            Element temp = CoalesceParser(ref c, Echo, Alias, Return, Expression);;
            if (CheckToken(c, TokenType.EndExpression))
            {
                SkipLineTerminator(++c);
            }
            return temp;
        }

        private EchoDirective Echo(ref int c)
        {
            if (!CheckText(c, "echo"))
            {
                return null;
            }
            SkipLineTerminator(++c);
            Element exp = Expression(ref c);
            return new EchoDirective { Exp = exp, Position = exp.Position };
        }

        private AliasDirective Alias(ref int c)
        {
            if (!CheckText(c, "alias"))
            {
                return null;
            }
            SkipLineTerminator(++c);
            var from = IdentifierAccess(ref c);
            var to = IdentifierAccess(ref c);
            return new AliasDirective { From = from, To = to, Position = from.Position };
        }

        private ReturnDirective Return(ref int c)
        {
            if (!CheckText(c, "return"))
            {
                return null;
            }
            SkipLineTerminator(++c);
            Element exp = Expression(ref c);
            return new ReturnDirective { Exp = exp, Position = exp.Position };
        }
    }
}
