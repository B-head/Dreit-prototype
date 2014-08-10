using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SyntacticAnalysis
{
    public partial class Parser
    {
        private static ProgramContext GlobalContext(SlimChainParser cp)
        {
            IReadOnlyList<Element> child = null;
            var result = cp.Begin
                .Call(c => child = c, icp => ExpressionList(icp, false))
                .End(tp => new ProgramContext(tp, child, false));
            return result;
        }

        private static ProgramContext BlockContext(SlimChainParser cp)
        {
            IReadOnlyList<Element> child = null;
            var result = cp.Begin
                .Call(c => child = c, icp => ExpressionList(icp, true))
                .End(tp => new ProgramContext(tp, child, false));
            return result;
        }

        private static ProgramContext DirectContext(SlimChainParser cp)
        {
            IReadOnlyList<Element> child = null;
            var isInline = true;
            return cp.Begin
                .Any(
                    icp => icp.Type(TokenType.EndExpression),
                    icp => icp.Transfer(e => child = new Element[] { e }, Expression).Lt().Opt.Type(TokenType.EndExpression),
                    icp => icp.Call(c => child = c, iicp => ExpressionList(iicp, true)).Self(() => isInline = false)
                )
                .End(tp => new ProgramContext(tp, child, isInline));
        }

        private static ProgramContext InlineContext(SlimChainParser cp)
        {
            IReadOnlyList<Element> child = null;
            var isInline = true;
            return cp.Begin
                .Any(
                    icp => icp.Opt.Type(TokenType.Separator).Type(TokenType.EndExpression),
                    icp => icp.Type(TokenType.Separator).Transfer(e => child = new Element[] { e }, Expression).Lt().Opt.Type(TokenType.EndExpression),
                    icp => icp.Opt.Type(TokenType.Separator).Call(c => child = c, iicp => ExpressionList(iicp, true)).Self(() => isInline = false)
                )
                .End(tp => new ProgramContext(tp, child, isInline));
        }

        private static ProgramContext ThanContext(SlimChainParser cp)
        {
            IReadOnlyList<Element> child = null;
            var isInline = true;
            return cp.Begin
                .Any(
                    icp => icp.Opt.Call(ThanSeparator).Type(TokenType.EndExpression),
                    icp => icp.Call(ThanSeparator).Transfer(e => child = new Element[] { e }, Expression).Lt().Opt.Type(TokenType.EndExpression),
                    icp => icp.Opt.Call(ThanSeparator).Call(c => child = c, iicp => ExpressionList(iicp, true)).Self(() => isInline = false)
                )
                .End(tp => new ProgramContext(tp, child, isInline));
        }

        private static void ThanSeparator(SlimChainParser cp)
        {
            cp.Any(
                icp => icp.Type(TokenType.Separator),
                icp => icp.Text("then")
            );
        }

        private static IReadOnlyList<Element> ExpressionList(SlimChainParser cp, bool isBlock)
        {
            var child = new List<Element>();
            InsideParser cond;
            if (isBlock)
            {
                cp = cp.Type(TokenType.LeftBrace);
                cond = icp => icp.Not.Type(TokenType.RightBrace);
            }
            else
            {
                cond = icp => icp.Readable();
            }
            cp.Opt.Call(ExpressionSeparators)
                .Any(
                    iicp => iicp.Transfer(e => child.Add(e), Expression),
                    iicp => iicp.AddError()
                )
                .Loop(cond, icp =>
                {
                    icp.Opt.Call(ExpressionSeparators)
                    .Opt.Any(
                        iicp => iicp.Transfer(e => child.Add(e), Expression),
                        iicp => iicp.AddError()
                    );
                })
                .Opt.Call(ExpressionSeparators);
            return child;
        }

        private static void ExpressionSeparators(SlimChainParser cp)
        {
            cp.Type(TokenType.EndExpression, TokenType.LineTerminator)
                .Ignore(TokenType.EndExpression, TokenType.LineTerminator);
        }
    }
}
