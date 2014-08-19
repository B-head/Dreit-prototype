using AbstractSyntax;
using AbstractSyntax.Expression;
using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.SyntacticAnalysis
{
    public partial class Parser
    {
        private static IfStatement IfStatement(SlimChainParser cp)
        {
            Element cond = null;
            ProgramContext than = null;
            ProgramContext els = null;
            return cp.Begin
                .Text("if").Lt()
                .Transfer(e => cond = e, Expression)
                .Transfer(e => than = e, ThanContext)
                .If(icp => icp.Text("else").Lt())
                .Than(icp => icp.Transfer(e => els = e, DirectContext))
                .End(tp => new IfStatement(tp, cond, than, els));
        }

        private static PatternMatchStatement PatternMatchStatement(SlimChainParser cp)
        {
            return null;
        }

        private static LoopStatement LoopStatement(SlimChainParser cp)
        {
            Element cond = null;
            Element use = null;
            Element by = null;
            ProgramContext block = null;
            return cp.Begin
                .Call(icp => LoopPart(icp, out cond, out use, out by))
                .Transfer(e => block = e, InlineContext)
                .End(tp => new LoopStatement(tp, false, cond, use, by, block));
        }

        private static LoopStatement LaterLoopStatement(SlimChainParser cp)
        {
            Element cond = null;
            Element use = null;
            Element by = null;
            ProgramContext block = null;
            return cp.Begin
                .Text("later")
                .Transfer(e => block = e, DirectContext)
                .Call(icp => LoopPart(icp, out cond, out use, out by))
                .End(tp => new LoopStatement(tp, true, cond, use, by, block));
        }

        private static void LoopPart(SlimChainParser cp, out Element cond, out Element use, out Element by)
        {
            Element c = null;
            Element u = null;
            Element b = null;
            cp.Text("loop").Lt()
                .Opt.Transfer(e => c = e, Expression)
                .If(icp => icp.Text("use").Lt())
                .Than(icp => icp.Transfer(e => u = e, DefaultValueVariantDeclaration))
                .If(icp => icp.Text("by").Lt())
                .Than(icp => icp.Transfer(e => b = e, Expression));
            cond = c;
            use = u;
            by = b;
        }

        private static ForStatement ForStatement(SlimChainParser cp)
        {
            Element cond = null;
            Element of = null;
            Element at = null;
            ProgramContext block = null;
            return cp.Begin
                .Text("for").Lt()
                .Opt.Transfer(e => cond = e, Expression)
                .If(icp => icp.Text("of").Lt())
                .Than(icp => icp.Transfer(e => of = e, DefaultValueVariantDeclaration))
                .If(icp => icp.Text("at").Lt())
                .Than(icp => icp.Transfer(e => at = e, DefaultValueVariantDeclaration))
                .Transfer(e => block = e, InlineContext)
                .End(tp => new ForStatement(tp, cond, of, at, block));
        }

        private static UnStatement UnStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("un").Lt()
                .Transfer(e => exp = e, Expression)
                .End(tp => new UnStatement(tp, exp));
        }

        private static LabelStatement LabelStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("label").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new LabelStatement(tp, exp));
        }

        private static GotoStatement GotoStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("goto").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new GotoStatement(tp, exp));
        }

        private static ContinueStatement ContinueStatement(SlimChainParser cp)
        {
            return cp.Begin
                .Text("continue").Lt()
                .End(tp => new ContinueStatement(tp));
        }

        private static BreakStatement BreakStatement(SlimChainParser cp)
        {
            return cp.Begin
                .Text("break").Lt()
                .End(tp => new BreakStatement(tp));
        }

        private static GiveStatement GiveStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("give").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new GiveStatement(tp, exp));
        }

        private static ReturnStatement ReturnStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("return").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new ReturnStatement(tp, exp));
        }

        private static YieldStatement YieldStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("yield").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new YieldStatement(tp, exp));
        }

        private static ThrowStatement ThrowStatement(SlimChainParser cp)
        {
            Element exp = null;
            return cp.Begin
                .Text("throw").Lt()
                .Opt.Transfer(e => exp = e, Expression)
                .End(tp => new ThrowStatement(tp, exp));
        }

        private static CatchStatement CatchStatement(SlimChainParser cp)
        {
            Element use = null;
            ProgramContext block = null;
            return cp.Begin
                .Text("catch")
                .Transfer(e => use = e, Expression)
                .Transfer(e => block = e, InlineContext)
                .End(tp => new CatchStatement(tp, use, block));
        }

        private static ScopeGuardStatement ScopeGuardStatement(SlimChainParser cp)
        {
            ScopeGuardType type = ScopeGuardType.Unknown;
            ProgramContext block = null;
            return cp.Begin
                .Any(
                    icp => icp.Text("exit").Self(() => type = ScopeGuardType.Exit),
                    icp => icp.Text("success").Self(() => type = ScopeGuardType.Success),
                    icp => icp.Text("failure").Self(() => type = ScopeGuardType.Failure)
                )
                .Transfer(e => block = e, DirectContext)
                .End(tp => new ScopeGuardStatement(tp, type, block));
        }

        private static RequireStatement RequireStatement(SlimChainParser cp)
        {
            ProgramContext block = null;
            return cp.Begin
                .Text("require")
                .Transfer(e => block = e, DirectContext)
                .End(tp => new RequireStatement(tp, block));
        }

        private static EnsureStatement EnsureStatement(SlimChainParser cp)
        {
            Element use = null;
            ProgramContext block = null;
            return cp.Begin
                .Text("ensure")
                .If(icp => icp.Transfer(e => use = e, Expression))
                .Than(icp => icp.Transfer(e => block = e, InlineContext))
                .Else(icp => icp.Transfer(e => block = e, DirectContext))
                .End(tp => new EnsureStatement(tp, use, block));
        }
    }
}
