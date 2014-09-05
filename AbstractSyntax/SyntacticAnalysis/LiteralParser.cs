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
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SyntacticAnalysis
{
    public partial class Parser
    {
        private static NumericLiteral NumericLiteral(SlimChainParser cp)
        {
            var integral = string.Empty;
            var fraction = string.Empty;
            return cp.Begin
                .Type(t => integral = t.Text, TokenType.DigitStartString).Lt()
                .If(icp => icp.Type(TokenType.Access).Lt())
                .Then(icp => icp.Type(t => fraction = t.Text, TokenType.DigitStartString).Lt())
                .End(tp => new NumericLiteral(tp, integral, fraction));
        }

        private static StringLiteral StringLiteral(SlimChainParser cp)
        {
            var texts = new List<Element>();
            var isEfficient = false;
            return cp.Begin
                .Any(
                    icp => icp.Type(TokenType.EfficientQuoteSeparator).Self(() => isEfficient = true),
                    icp => icp.Type(TokenType.QuoteSeparator).Self(() => isEfficient = false)
                )
                .Loop(icp => icp.Not.Type(TokenType.QuoteSeparator), icp =>
                {
                    icp
                    .If(iicp => iicp.Type(TokenType.LeftBrace))
                    .Then(iicp => iicp.Transfer(e => texts.Add(e), Expression).Type(TokenType.RightBrace))
                    .Else(iicp => iicp.Transfer(e => texts.Add(e), iiicp => PlainText(iiicp, isEfficient)));
                })
                .End(tp => new StringLiteral(tp, texts, isEfficient));
        }

        private static PlainText PlainText(SlimChainParser cp, bool isEfficient)
        {
            var value = string.Empty;
            return cp.Begin
                .Type(t => value = t.Text, TokenType.PlainText)
                .End(tp => new PlainText(tp, value, isEfficient));
        }

        private static HereDocument HereDocument(SlimChainParser cp)
        {
            return null;
        }

        private static RangeLiteral RangeLiteral(SlimChainParser cp)
        {
            Element left = null;
            Element right = null;
            bool isLeftOpen = false;
            bool isRightOpen = false;
            return cp.Begin
                .Any(
                    icp => icp.Type(TokenType.LeftParenthesis).Self(() => isLeftOpen = false),
                    icp => icp.Type(TokenType.LeftBracket).Self(() => isLeftOpen = true)
                )
                .Transfer(e => left = e, LeftCompose)
                .Type(TokenType.Range)
                .Transfer(e => right = e, LeftCompose)
                .Any(
                    icp => icp.Type(TokenType.RightParenthesis).Self(() => isLeftOpen = false),
                    icp => icp.Type(TokenType.RightBracket).Self(() => isLeftOpen = true)
                )
                .End(tp => new RangeLiteral(tp, left, right, isLeftOpen, isRightOpen));
        }

        private static Element NakedRangeLiteral(SlimChainParser cp)
        {
            Element left = null;
            Element right = null;
            var ret = cp.Begin
                .Transfer(e => left = e, LeftCompose)
                .Type(TokenType.Range)
                .Transfer(e => right = e, LeftCompose)
                .End(tp => new RangeLiteral(tp, left, right));
            return ret ?? LeftCompose(cp);
        }

        private static ArrayLiteral ArrayLiteral(SlimChainParser cp)
        {
            var values = new List<Element>();
            return cp.Begin
                .Type(TokenType.LeftBracket).Lt()
                .Loop(icp =>
                {
                    icp.Transfer(e => values.Add(e), NakedRangeLiteral)
                        .Type(TokenType.List).Lt();
                })
                .Type(TokenType.RightBracket).Lt()
                .End(tp => new ArrayLiteral(tp, values));
        }

        private static DictionaryLiteral DictionaryLiteral(SlimChainParser cp)
        {
            var pairs = new List<AssociatePair>();
            return cp.Begin
                .Type(TokenType.LeftBracket).Lt()
                .Loop(icp =>
                {
                    icp.Transfer(e => pairs.Add(e), AssociatePair)
                        .Type(TokenType.List).Lt();
                })
                .Type(TokenType.RightBracket).Lt()
                .End(tp => new DictionaryLiteral(tp, pairs));
        }

        private static AssociatePair AssociatePair(SlimChainParser cp)
        {
            Element left = null;
            Element right = null;
            return cp.Begin
                .Transfer(e => left = e, NakedRangeLiteral)
                .Type(TokenType.Pair)
                .Transfer(e => right = e, NakedRangeLiteral)
                .End(tp => new AssociatePair(tp, left, right));
        }

        private static TupleLiteral TupleLiteral(SlimChainParser cp)
        {
            var child = new List<Element>();
            return cp.Begin
                .Loop(icp =>
                {
                    icp.Transfer(e => child.Add(e), TuplePair)
                        .Type(TokenType.List).Lt();
                })
                .End(tp => new TupleLiteral(tp, child));
        }

        private static Element TupleExpression(SlimChainParser cp)
        {
            var child = new List<Element>();
            cp = cp.Begin
                .Loop(icp =>
                {
                    icp.Transfer(e => child.Add(e), TuplePair)
                        .Type(TokenType.List).Lt();
                });
            if (child.Count == 0)
            {
                return null;
            }
            else if (child.Count == 1)
            {
                return cp.End(tp => child[0]);
            }
            else
            {
                return cp.End(tp => new TupleLiteral(tp, child));
            }
        }

        private static Element TuplePair(SlimChainParser cp)
        {
            Identifier tag = null;
            Element exp = null;
            var ret = cp.Begin
                .Transfer(e => tag = e, Identifier)
                .Type(TokenType.Pair)
                .Transfer(e => exp = e, NakedRangeLiteral)
                .End(tp => new TuplePair(tp, tag, exp));
            return ret ?? NakedRangeLiteral(cp);
        }

        private static LambdaLiteral LambdaLiteral(SlimChainParser cp)
        {
            return null;
        }
    }
}
