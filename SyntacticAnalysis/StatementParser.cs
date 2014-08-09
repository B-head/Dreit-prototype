using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntacticAnalysis
{
    public partial class Parser
    {
        private static IfStatement IfStatement(SlimChainParser cp)
        {
            Element cond = null;
            ExpressionList than = null;
            ExpressionList els = null;
            return cp.Begin
                .Text("if").Lt()
                .Transfer(e => cond = e, Expression)
                .Transfer(e => than = e, icp => Block(icp, true))
                .If(icp => icp.Text("else").Lt())
                .Than(icp => icp.Transfer(e => els = e, iicp => Block(iicp, false)))
                .End(tp => new IfStatement(tp, cond, than, els));
        }

        private static LoopStatement LoopStatement(SlimChainParser cp)
        {
            Element cond = null;
            Element on = null;
            Element by = null;
            ExpressionList block = null;
            return cp.Begin
                .Text("loop").Lt()
                .Opt.Transfer(e => cond = e, Expression)
                .If(icp => icp.Text("on").Lt())
                .Than(icp => icp.Transfer(e => on = e, Expression))
                .If(icp => icp.Text("by").Lt())
                .Than(icp => icp.Transfer(e => by = e, Expression))
                .Transfer(e => block = e, icp => Block(icp, true))
                .End(tp => new LoopStatement(tp, cond, on, by, block));
        }

        private static UnStatement UnStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("un").Lt()
                .Transfer(e => exp = e, Expression)
                .End(tp => new UnStatement(tp, exp));
        }

        private static EchoStatement EchoStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("echo").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new EchoStatement(tp, exp));
        }

        private static ReturnStatement ReturnStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("return").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new ReturnStatement(tp, exp));
        }

        private static BreakStatement BreakStatement(SlimChainParser cp)
        {
            return cp.Begin
                .Text("break").Lt()
                .End(tp => new BreakStatement(tp));
        }

        private static ContinueStatement ContinueStatement(SlimChainParser cp)
        {
            return cp.Begin
                .Text("continue").Lt()
                .End(tp => new ContinueStatement(tp));
        }
    }
}
