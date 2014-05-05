using AbstractSyntax;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static SyntacticAnalysis.ParserFunction<Element>[] directive = { Echo, Alias, Return, LeftAssign }; 

        private static DirectiveList DirectiveList(ChainParser cp, bool root = false)
        {
            var result = cp.Begin<DirectiveList>()
                .If().Is(!root).Than().Type(TokenType.LeftBrace).EndIf()
                .Loop().Not().Type(TokenType.RightBrace).Than()
                .Ignore(TokenType.EndExpression, TokenType.LineTerminator)
                .Transfer((s, e) => s.Append(e), directive).Or().Error()
                .EndLoop().End();
            if(result == null)
            {
                result = new DirectiveList();
            }
            return result;
        }

        private static DirectiveList InlineDirectiveList(ChainParser cp)
        {
            return cp.Begin<DirectiveList>()
                .Type(TokenType.Separator).Transfer((s, e) => s.Append(e), directive)
                .End();
        }

        private static Element Directive(TokenCollection c, ref int i)
        {
            return null;
        }

        private static DirectiveList Block(TokenCollection c, ref int i)
        {
            return null;
        }

        private static EchoDirective Echo(ChainParser cp)
        {
            return cp.Begin<EchoDirective>()
                .Text("echo").Transfer((s, e) => s.Exp = e, directive)
                .End();
        }

        private static AliasDirective Alias(ChainParser cp)
        {
            return cp.Begin<AliasDirective>()
                .Text("alias").Transfer((s, e) => s.From = e, IdentifierAccess)
                .Transfer((s, e) => s.To = e, IdentifierAccess)
                .End();
        }

        private static ReturnDirective Return(ChainParser cp)
        {
            return cp.Begin<ReturnDirective>()
                .Text("return").Transfer((s, e) => s.Exp = e, directive)
                .End();
        }
    }
}
