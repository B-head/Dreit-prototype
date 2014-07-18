using AbstractSyntax;
using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static TransferParser<Element>[] directive = 
        { 
            AttributeZone,
            Echo,
            Alias,
            Return,
            Break,
            Continue,
            Expression
        };

        private static DirectiveList RootDirectiveList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var result = cp.Begin
                .Ignore(TokenType.EndExpression, TokenType.LineTerminator)
                .Loop(icp => icp.Readable(), icp =>
                {
                    icp.Any(
                        iicp => iicp.Transfer(e => child.Add(e), Directive),
                        iicp => iicp.AddError()
                    )
                    .Ignore(TokenType.EndExpression, TokenType.LineTerminator);
                })
                .End(tp => new DirectiveList(tp, child, false));
            if(result == null)
            {
                throw new InvalidOperationException();
            }
            return result;
        }

        private static DirectiveList BlockDirectiveList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var result = cp.Begin
                .Type(TokenType.LeftBrace)
                .Ignore(TokenType.EndExpression, TokenType.LineTerminator)
                .Loop(icp => icp.Not.Type(TokenType.RightBrace), icp =>
                {
                    icp.Any(
                        iicp => iicp.Transfer(e => child.Add(e), Directive),
                        iicp => iicp.AddError()
                    )
                    .Ignore(TokenType.EndExpression, TokenType.LineTerminator);
                })
                .End(tp => new DirectiveList(tp, child, false));
            if (result == null)
            {
                result = new DirectiveList();
            }
            return result;
        }

        private static DirectiveList InlineDirectiveList(SlimChainParser cp, bool separator)
        {
            var child = new List<Element>();
            return cp.Begin
                .If(icp => icp.Is(separator))
                .Than(icp => 
                {
                    icp.Any(
                        iicp => iicp.Type(TokenType.Separator),
                        iicp => iicp.Text("do"),
                        iicp => iicp.Text("then")
                    ).Lt();
                })
                .Transfer(e => child.Add(e), Directive)
                .End(tp => new DirectiveList(tp, child, true));
        }

        private static DirectiveList Block(SlimChainParser cp, bool separator)
        {
            return CoalesceParser<DirectiveList>(cp, icp => InlineDirectiveList(icp, separator), BlockDirectiveList);
        }

        private static Element Directive(SlimChainParser cp)
        {
            return CoalesceParser(cp, directive);
        }

        private static AttributeZoneDirective AttributeZone(SlimChainParser cp)
        {
            var child = new List<Element>();
            return cp.Begin
                .Type(TokenType.Zone).Lt()
                .Loop(icp =>
                {
                    icp
                    .Transfer(e => child.Add(e), IdentifierAccess)
                    .Type(TokenType.List).Lt();
                })
                .End(tp => new AttributeZoneDirective(tp, child));
        }

        private static EchoDirective Echo(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("echo").Lt()
                .Opt.Transfer(e => exp = e, Directive)
                .End(tp => new EchoDirective(tp, exp));
        }

        private static ReturnDirective Return(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("return").Lt()
                .Opt.Transfer(e => exp = e, Directive)
                .End(tp => new ReturnDirective(tp,exp));
        }

        private static AliasDirective Alias(SlimChainParser cp)
        {
            IdentifierAccess from = null;
            IdentifierAccess to = null;
            return cp.Begin
                .Text("alias").Lt()
                .Opt.Transfer(e => from = e, IdentifierAccess)
                .Opt.Transfer(e => to = e, IdentifierAccess)
                .End(tp => new AliasDirective(tp, from, to));
        }

        private static BreakDirective Break(SlimChainParser cp)
        {
            return cp.Begin
                .Text("break").Lt()
                .End(tp => new BreakDirective(tp));
        }

        private static ContinueDirective Continue(SlimChainParser cp)
        {
            return cp.Begin
                .Text("continue").Lt()
                .End(tp => new ContinueDirective(tp));
        }
    }
}
