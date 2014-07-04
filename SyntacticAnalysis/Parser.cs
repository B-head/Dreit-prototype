using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static ParserFunction<Element>[] directive = { AttributeZone, Echo, Alias, Return, Break, Continue, Expression };
        private static ParserFunction<Element>[] primary = { IfStatement, LoopStatement, NotStatement, DeclateClass, DeclateRoutine, DeclateOperator, DeclareVariant, ExpressionGroup, StringLiteral, NumberLiteral, IdentifierAccess };
        private static string[] attribute = { "static", "public", "protected", "private" };

        public static DeclateModule Parse(TokenCollection collection)
        {
            var cp = new ChainParser(collection);
            var exp = RootDirectiveList(cp);
            return new DeclateModule 
            { 
                Name = collection.GetName(),
                SourceText = collection.Text,
                ExpList = exp,
                ErrorToken = collection.ErrorToken,
                Position = collection.FirstPosition.AlterLength(collection.LastPosition),
            };
        }

        private static T CoalesceParser<T>(ChainParser cp, params ParserFunction<T>[] func) where T : Element
        {
            T result = null;
            foreach (var f in func)
            {
                result = f(cp);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        private static Element LeftAssociative<R, T>(ChainParser cp, ParserFunction<T> next, params TokenType[] type)
            where R : Element, IDyadicExpression, new()
            where T : Element
        {
            Element current = next(cp);
            return current == null ? null : LeftAssociative<R, T>(current, cp, next, type);
        }

        private static Element LeftAssociative<R, T>(Element current, ChainParser cp, ParserFunction<T> next, params TokenType[] type)
            where R : Element, IDyadicExpression, new()
            where T : Element
        {
            var ret = cp.Begin<R>()
                .Self(s => s.Left = current)
                .Type((s, t) => s.Operator = t.TokenType, type).Lt()
                .Transfer((s, e) => s.Right = e, next)
                .End();
            return ret == null ? current : LeftAssociative<R, T>(ret, cp, next, type);
        }

        private static Element RightAssociative<R, T>(ChainParser cp, ParserFunction<T> next, params TokenType[] type)
            where R : Element, IDyadicExpression, new()
            where T : Element
        {
            Element current = next(cp);
            var ret = cp.Begin<R>()
                .Self(s => s.Left = current)
                .Type((s, t) => s.Operator = t.TokenType, type).Lt()
                .Transfer((s, e) => s.Right = e, c => RightAssociative<R, T>(c, next, type))
                .End();
            return ret ?? current;
        }

        private static TupleList ParseTuple<T>(ChainParser cp, ParserFunction<T> next) where T : Element
        {
            return cp.Begin<TupleList>().Loop()
                .Transfer((s, e) => s.Append(e), next)
                .Type(TokenType.List).Lt()
                .Do().EndLoop().End();
        }

        private static IdentifierAccess TextToIdentifier(ChainParser cp, string text)
        {
            return cp.Begin<IdentifierAccess>()
                .Self(s => s.Value = text)
                .End();
        }
    }
}
