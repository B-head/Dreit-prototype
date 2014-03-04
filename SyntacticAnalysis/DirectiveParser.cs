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
                    SkipSpaser(++c);
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

        private Element Directive(ref int c)
        {
            Element temp = CoalesceParser(ref c, Echo, Expression);;
            if (CheckToken(c, TokenType.EndExpression))
            {
                SkipSpaser(++c);
            }
            return temp;
        }

        private Element Echo(ref int c)
        {
            if (!CheckText(c, "echo"))
            {
                return null;
            }
            SkipSpaser(++c);
            Element exp = Expression(ref c);
            return new EchoDirective { Exp = exp, Position = exp.Position };
        }
    }
}
