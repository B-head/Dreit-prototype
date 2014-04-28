using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        internal delegate Element ParserFunction(TokenCollection c, ref int i);

        public static Element Parse(TokenCollection collection)
        {
            int i = -1;
            collection.MoveNextToken(ref i);
            var p = collection.GetTextPosition(i);
            DirectiveList exp = DirectiveList(collection, ref i, true);
            if (exp.Count > 0)
            {
                exp.Position = p.AlterLength((TextPosition?)exp[exp.Count - 1]);
            }
            else
            {
                exp.Position = p;
            }
            return new DeclateModule { Name = collection.GetName(), SourceText = collection.Text, ExpList = exp, ErrorToken = collection.ErrorToken, Position = collection.LastPosition };
        }

        private static Element CoalesceParser(TokenCollection c, ref int i, params ParserFunction[] func)
        {
            Element result = null;
            foreach (ParserFunction f in func)
            {
                int temp = i;
                result = f(c, ref temp);
                if (result != null)
                {
                    i = temp;
                    break;
                }
            }
            return result;
        }

        private static Element LeftAssociative<R>(TokenCollection c, ref int i, ParserFunction next, params TokenType[] type) where R : DyadicExpression, new()
        {
            Element left = next(c, ref i);
            TokenType match;
            while (c.CheckToken(i, out match, type))
            {
                c.MoveNextToken(ref i);
                Element right = next(c, ref i);
                left = new R { Left = left, Right = right, Operator = match, Position = left.Position.AlterLength(right.Position) };
            }
            return left;
        }

        private static Element RightAssociative<R>(TokenCollection c, ref int i, ParserFunction next, params TokenType[] type) where R : DyadicExpression, new()
        {
            Element left = next(c, ref i);
            TokenType match;
            if (!c.CheckToken(i, out match, type))
            {
                return left;
            }
            c.MoveNextToken(ref i);
            Element right = RightAssociative<R>(c, ref i, next, type);
            return new R { Left = left, Right = right, Operator = match, Position = left.Position.AlterLength(right.Position) };
        }

        private static TupleList ParseTuple(TokenCollection c, ref int i, ParserFunction next)
        {
            TupleList tuple = new TupleList();
            while (c.IsReadable(i))
            {
                var temp = next(c, ref i);
                if(temp == null)
                {
                    break;
                }
                tuple.Append(temp);
                if (!c.CheckToken(i, TokenType.List))
                {
                    break;
                }
                c.MoveNextToken(ref i);
            }
            if(tuple.Count > 0)
            {
                tuple.Position = tuple[0].Position.AlterLength((TextPosition?)tuple[tuple.Count - 1]);
            }
            else
            {
                tuple.Position = c.GetTextPosition(i);
            }
            return tuple;
        }
    }
}
