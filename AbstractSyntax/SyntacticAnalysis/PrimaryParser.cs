/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
