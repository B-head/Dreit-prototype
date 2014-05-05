using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static DeclateVariant DeclareVariant(ChainParser cp)
        {
            return cp.Begin<DeclateVariant>()
                .Text("var").Lt()
                .Transfer((s, e) => s.Ident = e, IdentifierAccess)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, Addtive)
                .EndIf().End();
        }

        private static DeclateRoutine DeclateRoutine(ChainParser cp)
        {
            return cp.Begin<DeclateRoutine>()
                .Text("rout", "routine").Lt()
                .Type((s, t) => s.Name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer((s, e) => s.Generic = e, GenericList)
                .Transfer((s, e)=> s.Argument = e, ArgumentList)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, Addtive).EndIf()
                .Transfer((s, e)=> s.Block = e, Block)
                .End();
        }

        private static DeclateOperator DeclateOperator(ChainParser cp)
        {
            return cp.Begin<DeclateOperator>()
                .Text("operator").Lt()
                .Token((s, t) => { s.Operator = t.Type; s.Name = t.Text; }).Lt()
                .Transfer((s, e) => s.Generic = e, GenericList)
                .Transfer((s, e) => s.Argument = e, ArgumentList)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, Addtive).EndIf()
                .Transfer((s, e) => s.Block = e, Block)
                .End();
        }

        private static DeclateClass DeclateClass(ChainParser cp)
        {
            return cp.Begin<DeclateClass>()
                .Text("class").Lt()
                .Type((s, t) => s.Name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer((s, e) => s.Generic = e, GenericList)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.Inherit = e, c => ParseTuple(c, Addtive))
                .Else().Self(s => s.Inherit = new TupleList()).EndIf()
                .Transfer((s, e) => s.Block = e, Block)
                .End();
        }

        private static TupleList GenericList(ChainParser cp)
        {
            var ret = cp.Begin<TupleList>()
                .Type(TokenType.Not).Lt()
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop()
                .Transfer((s, e) => s.Append(e), DeclateGeneric)
                .Type(TokenType.List).Lt()
                .Than().EndLoop()
                .Type(TokenType.RightParenthesis).Lt()
                .End();
            return ret ?? new TupleList();
        }

        private static DeclateGeneric DeclateGeneric(ChainParser cp)
        {
            return cp.Begin<DeclateGeneric>()
                .Transfer((s, e) => s.Ident = e, IdentifierAccess)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.SpecializationType = e, Addtive)
                .EndIf().End();
        }

        private static TupleList ArgumentList(ChainParser cp)
        {
            var ret = cp.Begin<TupleList>()
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop()
                .Transfer((s, e) => s.Append(e), DeclateArgument)
                .Type(TokenType.List).Lt()
                .Than().EndLoop()
                .Type(TokenType.RightParenthesis).Lt()
                .End();
            return ret ?? new TupleList();
        }

        //todo デフォルト引数に対応した専用の構文が必要。
        private static DeclateArgument DeclateArgument(ChainParser cp)
        {
            return cp.Begin<DeclateArgument>()
                .Transfer((s, e) => s.Ident = e, IdentifierAccess)
                .If().Type(TokenType.Peir).Lt()
                .Than().Transfer((s, e) => s.ExplicitType = e, Addtive)
                .EndIf().End();
        }
    }
}
