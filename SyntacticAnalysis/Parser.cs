using AbstractSyntax;
using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private delegate Element MakeDyadicExpression(TextPosition tp, TokenType op, Element left, Element right);

        public static ModuleDeclaration Parse(TokenCollection collection)
        {
            var cp = new SlimChainParser(collection);
            var exp = RootExpressionList(cp);
            var tp = collection.FirstPosition.AlterLength(collection.LastPosition);
            return new ModuleDeclaration(tp, exp, collection.GetName(), collection.Text, collection.ErrorToken);
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

        private static Element LeftAssociative<T>(SlimChainParser cp, MakeDyadicExpression make, TransferParser<T> next, params TokenType[] type) where T : Element
        {
            Element current = next(cp);
            return current == null ? null : LeftAssociative<T>(current, cp, make, next, type);
        }

        private static Element LeftAssociative<T>(Element current, SlimChainParser cp, MakeDyadicExpression make, TransferParser<T> next, params TokenType[] type) where T : Element
        {
            var op = TokenType.Unknoun;
            Element right = null;
            var ret = cp.Begin
                .Type(t => op = t.TokenType, type).Lt()
                .Transfer(e => right = e, next)
                .End(tp => make(tp, op, current, right));
            return ret == null ? current : LeftAssociative<T>(ret, cp, make, next, type);
        }

        private static Element RightAssociative<T>(SlimChainParser cp, MakeDyadicExpression make, TransferParser<T> next, params TokenType[] type) where T : Element
        {
            var op = TokenType.Unknoun;
            Element right = null;
            Element current = next(cp);
            var ret = cp.Begin
                .Type(t => op = t.TokenType, type).Lt()
                .Transfer(e => right = e, icp => RightAssociative<T>(icp, make, next, type))
                .End(tp => make(tp, op, current, right));
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
                .End(tp => new TupleList(tp, child));
            if (result == null)
            {
                result = new TupleList();
            }
            return result;
        }
    }
}
