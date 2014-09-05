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
using AbstractSyntax;
using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;

namespace AbstractSyntax.SyntacticAnalysis
{
    public partial class Parser
    {
        private static string[] attribute = 
        { 
            "variadic",
            "final",
            "static",
            "public",
            "internal",
            "protected",
            "private",
        };

        private static ImportDeclaration ImportDeclaration(SlimChainParser cp)
        {
            List<Element> imports = new List<Element>();
            return cp.Begin
                .Text("import").Lt()
                .Loop(icp =>
                {
                    icp.Transfer(e => imports.Add(e), NakedRangeLiteral)
                        .Type(TokenType.List).Lt();
                })
                .End(tp => new ImportDeclaration(tp, imports));
        }

        private static AliasDeclaration AliasDeclaration(SlimChainParser cp)
        {
            Identifier from = null;
            Identifier to = null;
            return cp.Begin
                .Text("alias").Lt()
                .Opt.Transfer(e => to = e, Identifier)
                .Opt.Transfer(e => from = e, Identifier)
                .End(tp => new AliasDeclaration(tp, from, to));
        }

        private static VariantDeclaration VariantDeclaration(SlimChainParser cp)
        {
            var type = VariantType.Unknown;
            var name = string.Empty;
            TupleLiteral attr = null;
            Element expli = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Any(
                    icp => icp.Text("var").Self(() => type = VariantType.Var),
                    icp => icp.Text("let").Self(() => type = VariantType.Let),
                    icp => icp.Text("const").Self(() => type = VariantType.Const)
                ).Lt()
                .Type(t => name = t.Text, TokenType.LetterStartString)
                .If(icp => icp.Type(TokenType.Pair).Lt())
                .Then(icp => icp.Transfer(e => expli = e, Prefix))
                .End(tp => new VariantDeclaration(tp, type, name, attr, expli));
        }

        private static RoutineDeclaration RoutineDeclaration(SlimChainParser cp)
        {
            var type = RoutineType.Unknown;
            var opType = TokenType.Unknoun;
            var name = string.Empty;
            TupleLiteral attr = null;
            TupleLiteral generic = null;
            TupleLiteral args = null;
            Element expli = null;
            ProgramContext block = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Any(
                    icp => icp.Call(iicp => RoutinePrefix(iicp, out type, out name)),
                    icp => icp.Call(iicp => OperatorPrefix(iicp, out type, out opType, out name))
                )
                .Transfer(e => generic = e, GenericList)
                .Transfer(e => args = e, ArgumentList)
                .If(icp => icp.Type(TokenType.ReturnArrow).Lt())
                .Then(icp => icp.Transfer(e => expli = e, Prefix))
                .Transfer(e => block = e, InlineContext)
                .End(tp => new RoutineDeclaration(tp, name, type, opType, attr, generic, args, expli, block));
        }

        private static void RoutinePrefix(SlimChainParser cp, out RoutineType type, out string name)
        {
            var ty = RoutineType.Unknown;
            var n = string.Empty;
            cp.Any(
                    icp => icp.Text("rout", "routine").Self(() => ty = RoutineType.Routine),
                    icp => icp.Text("func", "function").Self(() => ty = RoutineType.Function)
                ).Lt()
                .Type(t => n = t.Text, TokenType.LetterStartString).Lt();
            type = ty;
            name = n;
        }

        private static void OperatorPrefix(SlimChainParser cp, out RoutineType type, out TokenType opType, out string name)
        {
            var ty = RoutineType.Routine;
            var oty = TokenType.Unknoun;
            var n = string.Empty;
            cp.Opt.Any(
                    icp => icp.Text("rout", "routine").Self(() => ty = RoutineType.Routine),
                    icp => icp.Text("func", "function").Self(() => ty = RoutineType.Function)
                ).Lt()
                .Text("operator")
                .Any(
                    icp => icp.Type(t => n = t.Text, TokenType.LetterStartString),
                    icp => icp.Take(t => { oty = t.TokenType; n = t.Text; })
                ).Lt();
            if(oty != TokenType.Unknoun)
            {
                switch(ty)
                {
                    case RoutineType.Routine: ty = RoutineType.RoutineOperator; break;
                    case RoutineType.Function: ty = RoutineType.FunctionOperator; break;
                    default: throw new ArgumentException();
                }
            }
            else
            {
                switch (ty)
                {
                    case RoutineType.Routine: ty = RoutineType.RoutineConverter; break;
                    case RoutineType.Function: ty = RoutineType.FunctionConverter; break;
                    default: throw new ArgumentException();
                }
            }
            type = ty;
            opType = oty;
            name = n;
        }

        private static ClassDeclaration ClassDeclaration(SlimChainParser cp)
        {
            var type = ClassType.Unknown;
            var name = string.Empty;
            TupleLiteral attr = null;
            TupleLiteral generic = null;
            TupleLiteral inherit = new TupleLiteral();
            ProgramContext block = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Any(
                    icp => icp.Text("class").Self(() => type = ClassType.Class),
                    icp => icp.Text("trait").Self(() => type = ClassType.Trait),
                    icp => icp.Text("extend").Self(() => type = ClassType.Extend)
                ).Lt()
                .Type(t => name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer(e => generic = e, GenericList)
                .If(icp => icp.Type(TokenType.Pair).Lt())
                .Then(icp => icp.Transfer(e => inherit = e, c => ParseTuple(c, Identifier)))
                .Transfer(e => block = e, InlineContext)
                .End(tp => new ClassDeclaration(tp, name, type, attr, generic, inherit, block));
        }

        private static EnumDeclaration EnumDeclaration(SlimChainParser cp)
        {
            var name = string.Empty;
            TupleLiteral attr = null;
            TupleLiteral generic = null;
            Element expli = null;
            ProgramContext block = null;
            return cp.Begin
                .Transfer(e => attr = e, AttributeList)
                .Text("enum").Lt()
                .Type(t => name = t.Text, TokenType.LetterStartString).Lt()
                .Transfer(e => generic = e, GenericList)
                .If(icp => icp.Type(TokenType.Pair).Lt())
                .Then(icp => icp.Transfer(e => expli = e, Prefix))
                .Transfer(e => block = e, EnumContext)
                .End(tp => new EnumDeclaration(tp, name, attr, generic, expli, block));
        }

        private static AlgebraDeclaration AlgebraDeclaration(SlimChainParser cp)
        {
            return null;
        }

        private static AttributeDeclaration AttributeDeclaration(SlimChainParser cp)
        {
            return null;
        }

        private static AttributeScope AttributeScope(SlimChainParser cp)
        {
            var child = new List<Element>();
            return cp.Begin
                .Type(TokenType.Zone).Lt()
                .Loop(icp =>
                {
                    icp
                    .Transfer(e => child.Add(e), Identifier)
                    .Type(TokenType.List).Lt();
                })
                .End(tp => new AttributeScope(tp, child));
        }

        private static TupleLiteral AttributeList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var atFlag = false;
            var ret = cp.Begin
                .Loop(icp =>
                {
                    icp
                    .If(iicp => iicp.Type(TokenType.Attribute).Lt())
                    .Then(iicp => { atFlag = true; iicp.Transfer(e => child.Add(e), Identifier); })
                    .ElseIf(iicp => iicp.Is(atFlag).Type(TokenType.List).Lt())
                    .Then(iicp => { atFlag = true; iicp.Transfer(e => child.Add(e), Identifier); })
                    .Else(iicp => { atFlag = false; iicp.Transfer(e => child.Add(e), iiicp => IdentifierMatch(iiicp, attribute)); });
                })
                .End(tp => new TupleLiteral(tp, child));
            return ret ?? new TupleLiteral();
        }

        private static TupleLiteral GenericList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var ret = cp.Begin
                .Type(TokenType.Template).Lt()
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop(icp =>
                {
                    icp
                    .Transfer(e => child.Add(e), GenericDeclaration)
                    .Type(TokenType.List).Lt();
                })
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new TupleLiteral(tp, child));
            return ret ?? new TupleLiteral();
        }

        private static GenericDeclaration GenericDeclaration(SlimChainParser cp)
        {
            var name = string.Empty;
            Element special = null;
            return cp.Begin
                .Type(t => name = t.Text, TokenType.LetterStartString)
                .If(icp => icp.Type(TokenType.Pair).Lt())
                .Then(icp => icp.Transfer(e => special = e, NakedRangeLiteral))
                .End(tp => new GenericDeclaration(tp, name, special));
        }

        private static TupleLiteral ArgumentList(SlimChainParser cp)
        {
            var child = new List<Element>();
            var ret = cp.Begin
                .Type(TokenType.LeftParenthesis).Lt()
                .Loop(icp =>
                {
                    icp
                    .Transfer(e => child.Add(e), ArgumentDeclaration)
                    .Type(TokenType.List).Lt();
                })
                .Type(TokenType.RightParenthesis).Lt()
                .End(tp => new TupleLiteral(tp, child));
            return ret ?? new TupleLiteral();
        }

        private static ArgumentDeclaration ArgumentDeclaration(SlimChainParser cp)
        {
            var type = VariantType.Var;
            var name = string.Empty;
            TupleLiteral attr = null;
            Element expli = null;
            Element def = null;
            return cp.Begin
                .Any(
                    icp => icp.Call(iicp => ArgumentPart(iicp, out type, out name, out attr, out expli)).Type(TokenType.LeftPipeline).Transfer(e => def = e, NakedRangeLiteral),
                    icp => icp.Transfer(e => def = e, NakedRangeLiteral).Type(TokenType.RightPipeline).Call(iicp => ArgumentPart(iicp, out type, out name, out attr, out expli)),
                    icp => icp.Call(iicp => ArgumentPart(iicp, out type, out name, out attr, out expli))
                )
                .End(tp => new ArgumentDeclaration(tp, type, name, attr, expli, def));
        }

        private static VariantDeclaration DefaultValueVariantDeclaration(SlimChainParser cp)
        {
            var type = VariantType.Var;
            var name = string.Empty;
            TupleLiteral attr = null;
            Element expli = null;
            Element def = null;
            return cp.Begin
                .Any(
                    icp => icp.Call(iicp => ArgumentPart(iicp, out type, out name, out attr, out expli)).Type(TokenType.LeftPipeline).Transfer(e => def = e, NakedRangeLiteral),
                    icp => icp.Transfer(e => def = e, NakedRangeLiteral).Type(TokenType.RightPipeline).Call(iicp => ArgumentPart(iicp, out type, out name, out attr, out expli)),
                    icp => icp.Call(iicp => ArgumentPart(iicp, out type, out name, out attr, out expli)).Self(() => def = null)
                )
                .End(tp => new VariantDeclaration(tp, type, name, attr, expli, def));
        }

        private static void ArgumentPart(SlimChainParser cp, out VariantType type, out string name, out TupleLiteral attr, out Element expli)
        {
            var ty = VariantType.Var;
            var n = string.Empty;
            TupleLiteral a = null;
            Element ex = null;
            cp.Transfer(e => a = e, AttributeList)
                .Opt.Any(
                    icp => icp.Text("var").Self(() => ty = VariantType.Var),
                    icp => icp.Text("let").Self(() => ty = VariantType.Let),
                    icp => icp.Text("const").Self(() => ty = VariantType.Const)
                ).Lt()
                .Type(t => n = t.Text, TokenType.LetterStartString).Lt()
                .If(icp => icp.Type(TokenType.Pair).Lt())
                .Then(icp => icp.Transfer(e => ex = e, Prefix));
            type = ty;
            name = n;
            attr = a;
            expli = ex;
        }

        private static VariantDeclaration EnumField(SlimChainParser cp)
        {
            var name = string.Empty;
            TupleLiteral attr = null;
            Element def = null;
            return cp.Begin
                .Any(
                    icp => icp.Call(iicp => EnumFieldPart(iicp, out name, out attr)).Type(TokenType.LeftPipeline).Transfer(e => def = e, NakedRangeLiteral),
                    icp => icp.Transfer(e => def = e, NakedRangeLiteral).Type(TokenType.RightPipeline).Call(iicp => EnumFieldPart(iicp, out name, out attr)),
                    icp => icp.Call(iicp => EnumFieldPart(iicp, out name, out attr))
                )
                .End(tp => new VariantDeclaration(tp, VariantType.Const, name, attr, null, def));
        }

        private static void EnumFieldPart(SlimChainParser cp, out string name, out TupleLiteral attr)
        {
            var n = string.Empty;
            TupleLiteral a = null;
            cp.Transfer(e => a = e, AttributeList)
                .Type(t => n = t.Text, TokenType.LetterStartString).Lt();
            name = n;
            attr = a;
        }
    }
}
