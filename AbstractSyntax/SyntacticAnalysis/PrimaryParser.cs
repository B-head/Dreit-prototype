using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Statement;
using System.Collections.Generic;

namespace AbstractSyntax.SyntacticAnalysis
{
    public partial class Parser
    {
        private static TransferParser<Element>[] primary = 
        { 
            ImportDeclaration,
            AliasDeclaration,
            VariantDeclaration,
            RoutineDeclaration,
            ClassDeclaration,
            EnumDeclaration,
            AlgebraDeclaration,
            AttributeDeclaration,
            AttributeScope,
            IfStatement,
            PatternMatchStatement,
            LoopStatement,
            LaterLoopStatement,
            UnStatement,
            LabelStatement,
            GotoStatement,
            ContinueStatement,
            BreakStatement,
            GiveStatement,
            ReturnStatement,
            YieldStatement,
            ThrowStatement,
            CatchStatement,
            ScopeGuardStatement,
            RequireStatement,
            EnsureStatement,
            NumericLiteral,
            StringLiteral,
            HereDocument,
            RangeLiteral,
            ArrayLiteral,
            DictionaryLiteral,
            LambdaLiteral,
            GroupingExpression,
            Identifier
        };

        private static Element Primary(SlimChainParser cp)
        {
            return CoalesceParser(cp, primary);
        }

        private static GroupingExpression GroupingExpression(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Type(TokenType.LeftParenthesis).Lt()
                .Transfer(e => exp = e, Expression)
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new GroupingExpression(tp, exp));
        }

        private static Identifier Identifier(SlimChainParser cp)
        {
            var identType = TokenType.Unknoun;
            var value = string.Empty;
            return cp.Begin
                .Opt.Type(t => identType = t.TokenType, TokenType.Pragma, TokenType.Macro, TokenType.Nullable).Lt()
                .Type(t => value = t.Text, TokenType.LetterStartString).Lt()
                .End(tp => new Identifier(tp, value, identType));
        }

        private static Identifier IdentifierMatch(SlimChainParser cp, params string[] match)
        {
            var value = string.Empty;
            return cp.Begin
                .Text(t => value = t.Text, match).Lt()
                .End(tp => new Identifier(tp, value, TokenType.Unknoun));
        }
    }
}
