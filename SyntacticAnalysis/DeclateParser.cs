using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
using System.Collections.Generic;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static string[] attribute = 
        { 
            "static",
            "public",
            "protected",
            "private",
        };

        private static DeclateVariant DeclareVariant(SlimChainParser cp)
        {
            TupleList attr = null;
            var isLet = false;
            IdentifierAccess ident = null;
            IdentifierAccess expl = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Any(
                    icp => icp.Text(e => isLet = false, "var"),
                    icp => icp.Text(e => isLet = true, "let")
                ).Lt()
                .Transfer(e => ident = e, IdentifierAccess)
                .If(icp => icp.Type(TokenType.Peir).Lt())
                .Than(icp => icp.Transfer(e => expl = e, IdentifierAccess))
                .End(tp => new DeclateVariant(tp, attr, ident, expl, isLet));
        }

        private static DeclateRoutine DeclateRoutine(SlimChainParser cp)
        {
            TupleList attr = null;
            var isFunc = false;
            var name = string.Empty;
            TupleList generic = null;
            TupleList args = null;
            Element expl = null;
            DirectiveList block = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Any(
                    icp => icp.Text(e => isFunc = false, "rout", "routine"),
                    icp => icp.Text(e => isFunc = true, "func", "function")
                ).Lt()
                .Type(t => name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer(e => generic = e, GenericList)
                .Transfer(e => args = e, ArgumentList)
                .If(icp => icp.Type(TokenType.Peir).Lt())
                .Than(icp => icp.Transfer(e => expl = e, NonTupleExpression))
                .Transfer(e => block = e, icp => Block(icp, true))
                .End(tp => new DeclateRoutine(tp, name, TokenType.Unknoun, isFunc, attr, generic, args, expl, block));
        }

        private static DeclateRoutine DeclateOperator(SlimChainParser cp)
        {
            TupleList attr = null;
            var op = TokenType.Unknoun;
            var name = string.Empty;
            TupleList generic = null;
            TupleList args = null;
            Element expl = null;
            DirectiveList block = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Text("operator").Lt()
                .Take(t => { op = t.TokenType; name = t.Text; }).Lt()
                .Transfer(e => generic = e, GenericList)
                .Transfer(e => args = e, ArgumentList)
                .If(icp => icp.Type(TokenType.Peir).Lt())
                .Than(icp => icp.Transfer(e => expl = e, NonTupleExpression))
                .Transfer(e => block = e, icp => Block(icp, true))
                .End(tp => new DeclateRoutine(tp, name, op, false, attr, generic, args, expl, block));
        }

        private static DeclateClass DeclateClass(SlimChainParser cp)
        {
            TupleList attr = null;
            var isTrait = false;
            var name = string.Empty;
            TupleList generic = null;
            TupleList inherit = new TupleList();
            DirectiveList block = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Any(
                    icp => icp.Text(e => isTrait = false, "class"),
                    icp => icp.Text(e => isTrait = true, "trait")
                ).Lt()
                .Type(t => name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer(e => generic = e, GenericList)
                .If(icp => icp.Type(TokenType.Peir).Lt())
                .Than(icp => icp.Transfer(e => inherit = e, c => ParseTuple(c, IdentifierAccess)))
                .Transfer(e => block = e, icp => Block(icp, true))
                .End(tp => new DeclateClass(tp, name, isTrait, attr, generic, inherit, block));
        }

        private static TupleList AttributeList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var atFlag = false;
            var ret = cp.Begin
                .Loop(icp =>
                {
                    icp
                    .If(iicp => iicp.Type(TokenType.Attribute).Lt())
                    .Than(iicp => { atFlag = true; iicp.Transfer(e => child.Add(e), IdentifierAccess); })
                    .ElseIf(iicp => iicp.Is(atFlag).Type(TokenType.List).Lt())
                    .Than(iicp => { atFlag = true; iicp.Transfer(e => child.Add(e), IdentifierAccess); })
                    .Else(iicp => { atFlag = false; iicp.Transfer(e => child.Add(e), iiicp => IdentifierAccess(iiicp, attribute)); });
                })
                .End(tp => new TupleList(tp, child));
            return ret ?? new TupleList();
        }

        private static TupleList GenericList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var ret = cp.Begin
                .Type(TokenType.Template).Lt()
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop(icp =>
                {
                    icp
                    .Transfer(e => child.Add(e), DeclateGeneric)
                    .Type(TokenType.List).Lt();
                })
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new TupleList(tp, child));
            return ret ?? new TupleList();
        }

        private static DeclateGeneric DeclateGeneric(SlimChainParser cp)
        {
            var name = string.Empty;
            Element special = null;
            return cp.Begin
                .Type(t => name = t.Text, TokenType.LetterStartString)
                .If(icp => icp.Type(TokenType.Peir).Lt())
                .Than(icp => icp.Transfer(e => special = e, NonTupleExpression))
                .End(tp => new DeclateGeneric(tp, name, special));
        }

        private static TupleList ArgumentList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var ret = cp.Begin
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop(icp =>
                {
                    icp
                    .Transfer(e => child.Add(e), DeclateArgument)
                    .Type(TokenType.List).Lt();
                })
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new TupleList(tp, child));
            return ret ?? new TupleList();
        }

        //todo デフォルト引数に対応した専用の構文が必要。
        private static DeclateArgument DeclateArgument(SlimChainParser cp)
        {
            TupleList attr = null;
            IdentifierAccess ident = null;
            IdentifierAccess expl = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Transfer(e => ident = e, IdentifierAccess)
                .If(icp => icp.Type(TokenType.Peir).Lt())
                .Than(icp => icp.Transfer(e => expl = e, IdentifierAccess))
                .End(tp => new DeclateArgument(tp, attr, ident, expl));
        }
    }
}
