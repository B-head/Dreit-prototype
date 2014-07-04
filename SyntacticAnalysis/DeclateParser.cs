using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static DeclateVariant DeclareVariant(ChainParser cp) //todo 関数呼び出しタイプの初期化に対応した構文が必要。
        {
            return cp.Begin<DeclateVariant>()
                .Transfer((s, e) => s.AttributeAccess = e, AttributeList)
                .Text((s, e) => s.IsLet = false, "var")
                .Or().Text((s, e) => s.IsLet = true, "let").Lt()
                .Transfer((s, e) => s.Ident = e, IdentifierAccess)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, IdentifierAccess)
                .EndIf().End();
        }

        private static DeclateRoutine DeclateRoutine(ChainParser cp)
        {
            return cp.Begin<DeclateRoutine>()
                .Transfer((s, e) => s.AttributeAccess = e, AttributeList)
                .Text((s, e) => s.IsFunction = false, "rout", "routine")
                .Or().Text((s, e) => s.IsFunction = true, "func", "function").Lt()
                .Type((s, t) => s.Name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer((s, e) => s.DecGeneric = e, GenericList)
                .Transfer((s, e) => s.DecArguments = e, ArgumentList)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, NonTupleExpression).EndIf()
                .Transfer((s, e) => s.Block = e, Block)
                .End();
        }

        private static DeclateRoutine DeclateOperator(ChainParser cp)
        {
            return cp.Begin<DeclateRoutine>()
                .Transfer((s, e) => s.AttributeAccess = e, AttributeList)
                .Text("operator").Lt()
                .Token((s, t) => { s.Operator = t.TokenType; s.Name = t.Text; }).Lt()
                .Transfer((s, e) => s.DecGeneric = e, GenericList)
                .Transfer((s, e) => s.DecArguments = e, ArgumentList)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, NonTupleExpression).EndIf()
                .Transfer((s, e) => s.Block = e, Block)
                .End();
        }

        private static DeclateClass DeclateClass(ChainParser cp)
        {
            return cp.Begin<DeclateClass>()
                .Transfer((s, e) => s.AttributeAccess = e, AttributeList)
                .Text("class", "trait").Lt()
                .Type((s, t) => s.Name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer((s, e) => s.DecGeneric = e, GenericList)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.InheritAccess = e, c => ParseTuple(c, IdentifierAccess))
                .Else().Self(s => s.InheritAccess = new TupleList()).EndIf()
                .Transfer((s, e) => s.Block = e, Block)
                .End();
        }

        private static TupleList AttributeList(ChainParser cp)
        {
            var atFlag = false;
            var ret = cp.Begin<TupleList>()
                .Loop()
                .If().Type(TokenType.Attribute).Lt()
                .Than().Token((s, e) => s.Append(TextToIdentifier(cp, e.Text))).Self(s => atFlag = true)
                .ElseIf().Is(atFlag).Type(TokenType.List)
                .Than().Token((s, e) => s.Append(TextToIdentifier(cp, e.Text))).Self(s => atFlag = true)
                .Else().Text((s, e) => s.Append(TextToIdentifier(cp, e.Text)), attribute).Self(s => atFlag = false)
                .EndIf().Lt()
                .Do().EndLoop()
                .End();
            return ret ?? new TupleList();
        }

        private static TupleList GenericList(ChainParser cp)
        {
            var ret = cp.Begin<TupleList>()
                .Type(TokenType.Not).Lt()
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop()
                .Transfer((s, e) => s.Append(e), DeclateGeneric)
                .Type(TokenType.List).Lt()
                .Do().EndLoop()
                .Type(TokenType.RightParenthesis).Lt()
                .End();
            return ret ?? new TupleList();
        }

        private static DeclateGeneric DeclateGeneric(ChainParser cp)
        {
            return cp.Begin<DeclateGeneric>()
                .Type((s, t)=> s.Name = t.Text, TokenType.LetterStartString)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.SpecialTypeAccess = e, NonTupleExpression)
                .EndIf().End();
        }

        private static TupleList ArgumentList(ChainParser cp)
        {
            var ret = cp.Begin<TupleList>()
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop()
                .Transfer((s, e) => s.Append(e), DeclateArgument)
                .Type(TokenType.List).Lt()
                .Do().EndLoop()
                .Type(TokenType.RightParenthesis).Lt()
                .End();
            return ret ?? new TupleList();
        }

        //todo デフォルト引数に対応した専用の構文が必要。
        private static DeclateArgument DeclateArgument(ChainParser cp)
        {
            return cp.Begin<DeclateArgument>()
                .Transfer((s, e) => s.AttributeAccess = e, AttributeList)
                .Transfer((s, e) => s.Ident = e, IdentifierAccess)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, IdentifierAccess)
                .EndIf().End();
        }
    }
}
