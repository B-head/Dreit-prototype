using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        public static DeclateModule Parse(TokenCollection collection)
        {
            var cp = new SlimChainParser(collection);
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

        private static T CoalesceParser<T>(SlimChainParser cp, params TransferParser<T>[] func) where T : Element
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

        private static Element LeftAssociative<R, T>(SlimChainParser cp, TransferParser<T> next, params TokenType[] type)
            where R : Element, IDyadicExpression, new()
            where T : Element
        {
            Element current = next(cp);
            return current == null ? null : LeftAssociative<R, T>(current, cp, next, type);
        }

        private static Element LeftAssociative<R, T>(Element current, SlimChainParser cp, TransferParser<T> next, params TokenType[] type)
            where R : Element, IDyadicExpression, new()
            where T : Element
        {
            var op = TokenType.Unknoun;
            Element right = null;
            var ret = cp.Begin
                .Type(t => op = t.TokenType, type).Lt()
                .Transfer(e => right = e, next)
                .End(tp => new R { Left = current, Right = right, Operator = op, Position = tp});
            return ret == null ? current : LeftAssociative<R, T>(ret, cp, next, type);
        }

        private static Element RightAssociative<R, T>(SlimChainParser cp, TransferParser<T> next, params TokenType[] type)
            where R : Element, IDyadicExpression, new()
            where T : Element
        {
            var op = TokenType.Unknoun;
            Element right = null;
            Element current = next(cp);
            var ret = cp.Begin
                .Type(t => op = t.TokenType, type).Lt()
                .Transfer(e => right = e, icp => RightAssociative<R, T>(icp, next, type))
                .End(tp => new R { Left = current, Right = right, Operator = op, Position = tp });
            return ret ?? current;
        }

        private static TupleList ParseTuple<T>(SlimChainParser cp, TransferParser<T> next) where T : Element
        {
            var child = new List<Element>();
            var result = cp.Begin
                .Loop(icp =>
                {
                    icp
                    .Transfer(e => child.Add(e), next)
                    .Type(TokenType.List).Lt();
                })
                .End(tp => new TupleList(child) { Position = tp });
            if (result == null)
            {
                result = new TupleList();
            }
            return result;
        }
    }
}
