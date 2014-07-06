using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using AbstractSyntax.Statement;
using AbstractSyntax.Literal;
using AbstractSyntax.Directive;

namespace CliTranslate
{
    public class TranslateManager
    {
        private Dictionary<Scope, Translator> TransDictionary;
        private RootTranslator Root;

        public TranslateManager(string name, string dir = null)
        {
            TransDictionary = new Dictionary<Scope, Translator>();
            Root = new RootTranslator(name, dir);
        }

        public void Save()
        {
            Root.Save();
        }

        public void Run()
        {
            Root.Run();
        }

        public void TranslateTo(Root root, ImportManager manager)
        {
            manager.TranslateImport(Root);
            TransDictionary.Add(root, Root);
            ChildSpreadTranslate(root);
            Translate(root, Root);
            Root.BuildCode();
        }

        private void ChildSpreadTranslate(Scope scope)
        {
            foreach (var v in scope.ScopeChild)
            {
                if (v == null)
                {
                    continue;
                }
                RelaySpreadTranslate(v);
                ChildSpreadTranslate(v);
            }
        }

        private void RelaySpreadTranslate(Scope scope)
        {
            if (TransDictionary.ContainsKey(scope))
            {
                return;
            }
            RelaySpreadTranslate(scope.CurrentIScope);
            SpreadTranslate((dynamic)scope, TransDictionary[scope.CurrentIScope]);
        }

        private void SpreadTranslate(Scope scope, Translator trans)
        {
            TransDictionary.Add(scope, trans);
        }

        private void SpreadTranslate(DeclateModule scope, Translator trans)
        {
            var temp = trans.CreateModule(scope);
            TransDictionary.Add(scope, temp);
        }

        private void SpreadTranslate(DeclateClass scope, Translator trans)
        {
            foreach(var v in scope.Inherit)
            {
                RelaySpreadTranslate(v);
            }
            if (scope.IsPrimitive)
            {
                var temp = trans.CreatePrimitive(scope);
                TransDictionary.Add(scope, temp);
            }
            else
            {
                var temp = trans.CreateClass(scope);
                TransDictionary.Add(scope, temp);
            }
        }

        private void SpreadTranslate(DeclateRoutine scope, Translator trans)
        {
            RelaySpreadTranslate(scope.CallReturnType);
            foreach(var v in scope.ArgumentTypes)
            {
                RelaySpreadTranslate(v);
            }
            RoutineTranslator temp;
            if (scope.IsConstructor)
            {
                var cls = (ClassTranslator)trans;
                temp = cls.CreateConstructor(scope, scope.ArgumentTypes);
            }
            else if (scope.IsDestructor)
            {
                var cls = (ClassTranslator)trans;
                temp = cls.CreateDestructor(scope);
            }
            else
            {
                temp = trans.CreateRoutine(scope);
            }
            TransDictionary.Add(scope, temp);
            temp.CreateArguments(scope.DecArguments.Cast<Scope>());
        }

        private void SpreadTranslate(DeclateVariant scope, Translator trans)
        {
            RelaySpreadTranslate(scope.ReturnType);
            trans.CreateVariant(scope);
            TransDictionary.Add(scope, trans);
        }

        private void SpreadTranslate(IfStatement scope, Translator trans)
        {
            var t = trans.CreateBranch(scope, scope.IsDefinedElse);
            TransDictionary.Add(scope, t);
        }

        private void SpreadTranslate(LoopStatement scope, Translator trans)
        {
            var t = trans.CreateLoop(scope);
            TransDictionary.Add(scope, t);
        }

        private void ChildTranslate(Element element, Translator trans)
        {
            foreach(var v in element)
            {
                if (v == null)
                {
                    continue;
                }
                Translate((dynamic)v, trans);
            }
        }

        private void Translate(Element element, Translator trans)
        {
            ChildTranslate(element, trans);
        }

        private void Translate(DirectiveList element, Translator trans)
        {
            if (element.IsNoReturn)
            {
                ChildTranslate(element, trans);
                return;
            }
            foreach (var v in element)
            {
                Translate((dynamic)v, trans);
                if (!v.IsVoidReturn && !element.IsInline)
                {
                    trans.GenerateControl(OpCodes.Pop);
                }
            }
            if (element.Count <= 0 || !(element[element.Count - 1] is ReturnDirective))
            {
                trans.GenerateControl(OpCodes.Ret);
            }
        }

        private void Translate(DeclateModule element, Translator trans)
        {
            var temp = TransDictionary[element];
            ChildTranslate(element, temp);
        }

        private void Translate(DeclateClass element, Translator trans)
        {
            var temp = TransDictionary[element];
            Translate((dynamic)element.Block, temp);
        }

        private void Translate(DeclateRoutine element, Translator trans)
        {
            var temp = TransDictionary[element];
            Translate((dynamic)element.Block, temp);
            if (element.IsDefaultThisReturn)
            {
                temp.GenerateControl(OpCodes.Ldarg_0);
                temp.GenerateControl(OpCodes.Ret);
            }
        }

        private void Translate(DeclateVariant element, Translator trans)
        {
            Translate((dynamic)element.Ident, trans);
        }

        private void Translate(UnStatement element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateControl(OpCodes.Ldc_I4_1);
            trans.GenerateControl(OpCodes.Xor);
        }

        private void Translate(IfStatement element, Translator trans)
        {
            var bt = (BranchTranslator)TransDictionary[element];
            Translate((dynamic)element.Condition, trans);
            if (element.IsDefinedElse)
            {
                trans.GenerateJump(OpCodes.Brfalse, bt.ElseLabel);
                trans.BeginScope();
                Translate(element.Than, bt);
                trans.EndScope();
                trans.GenerateJump(OpCodes.Br, bt.EndLabel);
                trans.MarkLabel(bt.ElseLabel);
                trans.BeginScope();
                Translate(element.Else, bt);
                trans.EndScope();
                trans.MarkLabel(bt.EndLabel);
            }
            else
            {
                trans.GenerateJump(OpCodes.Brfalse, bt.ElseLabel);
                trans.BeginScope();
                Translate(element.Than, bt);
                trans.EndScope();
                trans.MarkLabel(bt.ElseLabel);
            }
        }

        private void Translate(LoopStatement element, Translator trans)
        {
            var lt = (LoopTranslator)TransDictionary[element];
            var bp = trans.CreateLabel();
            if (element.IsDefinedOn)
            {
                Translate((dynamic)element.On, lt);
                if (!element.On.IsVoidReturn)
                {
                    trans.GenerateControl(OpCodes.Pop);
                }
            }
            trans.GenerateJump(OpCodes.Br, bp);
            trans.MarkLabel(lt.GetContinueLabel());
            if (element.IsDefinedBy)
            {
                Translate((dynamic)element.By, lt);
                if (!element.By.IsVoidReturn)
                {
                    trans.GenerateControl(OpCodes.Pop);
                }
            }
            trans.MarkLabel(bp);
            if (element.IsDefinedCondition)
            {
                Translate((dynamic)element.Condition, lt);
                trans.GenerateJump(OpCodes.Brfalse, lt.GetBreakLabel());
            }
            Translate((dynamic)element.Block, lt);
            trans.GenerateJump(OpCodes.Br, lt.GetContinueLabel());
            trans.MarkLabel(lt.GetBreakLabel());
        }

        private void Translate(ContinueDirective element, Translator trans)
        {
            trans.GenerateJump(OpCodes.Br, trans.GetContinueLabel());
        }

        private void Translate(BreakDirective element, Translator trans)
        {
            trans.GenerateJump(OpCodes.Br, trans.GetBreakLabel());
        }

        private void Translate(ReturnDirective element, Translator trans)
        {
            Translate((dynamic)element.Exp, trans);
            trans.GenerateControl(OpCodes.Ret);
        }

        private void Translate(EchoDirective element, Translator trans)
        {
            Translate((dynamic)element.Exp, trans);
            trans.GenerateEcho(element.Exp.ReturnType);
        }

        private void Translate(NumberLiteral element, Translator trans)
        {
            dynamic number = element.Parse();
            trans.GeneratePrimitive(number);
        }

        private void Translate(StringLiteral element, Translator trans)
        {
            if(element.Texts.Count == 0)
            {
                trans.GeneratePrimitive(string.Empty);
                return;
            }
            var c = element.Texts.Count;
            for(var i = 0; i < c; ++i)
            {
                Translate((dynamic)element.Texts[i], trans);
                trans.GenerateToString(element.Texts[i].ReturnType);
                if(i > 0)
                {
                    trans.GenerateStringConcat();
                }
            }
        }

        private void Translate(PlainText element, Translator trans)
        {
            trans.GeneratePrimitive(element.ShowValue);
        }

        private void Translate(Calculate element, Translator trans)
        {
            Translate((dynamic)element.Left, trans);
            Translate((dynamic)element.Right, trans);
            trans.GenerateCall(element.CallScope);
        }

        private void Translate(Condition element, Translator trans)
        {
            Translate((dynamic)element.Left, trans);
            if (element.IsConnection)
            {
                Translate((dynamic)element.VirtualRight, trans);
                trans.GenerateCall(element.CallScope);
                Translate((dynamic)element.Right, trans);
                trans.GenerateControl(OpCodes.And);
            }
            else
            {
                Translate((dynamic)element.Right, trans);
                trans.GenerateCall(element.CallScope);
            }
        }

        private void Translate(Logical element, Translator trans)
        {
            var bend = trans.CreateLabel();
            if(element.Operator == TokenType.Or)
            {
                Translate((dynamic)element.Left, trans);
                trans.GenerateControl(OpCodes.Dup);
                trans.GenerateJump(OpCodes.Brtrue, bend);
                trans.GenerateControl(OpCodes.Pop);
                Translate((dynamic)element.Right, trans);
                trans.MarkLabel(bend);
            }
            else if (element.Operator == TokenType.And)
            {
                Translate((dynamic)element.Left, trans);
                trans.GenerateControl(OpCodes.Dup);
                trans.GenerateJump(OpCodes.Brfalse, bend);
                trans.GenerateControl(OpCodes.Pop);
                Translate((dynamic)element.Right, trans);
                trans.MarkLabel(bend);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void Translate(Prefix element, Translator trans)
        {
            Translate((dynamic)element.Child, trans);
        }

        private void Translate(Postfix element, Translator trans)
        {
            return; //todo 参照や型情報を返すようにする。
        }

        private void Translate(IdentifierAccess element, Translator trans)
        {
            if (element.IsTacitThis)
            {
                trans.GenerateLoad(element.ThisReference);
            }
            CallTranslate((dynamic)element.CallScope, new TupleList(), trans);
        }

        private void Translate(MemberAccess element, Translator trans)
        {
            Translate((dynamic)element.Access, trans);
            CallTranslate((dynamic)element.CallScope, new TupleList(), trans);
        }

        private void Translate(CallRoutine element, Translator trans)
        {
            if (element.IsCalculate)
            {
                var acs = TranslateAccess((dynamic)element.Access, trans);
                if (acs && element.CallScope is VariantSymbol && element.Arguments.Count > 0)
                {
                    trans.GenerateControl(OpCodes.Dup);
                }
                Translate((dynamic)element.Left, trans);
                Translate((dynamic)element.Right, trans);
                trans.GenerateCall(element.CalculateCallScope);
                CallTranslate((dynamic)element.CallScope, new TupleList(new TupleList()), trans); //todo CallTlanslateにArgumentsを渡さなくても済むようにする。
            }
            else
            {
                var acs = TranslateAccess((dynamic)element.Access, trans);
                if (acs && element.CallScope is VariantSymbol && element.Arguments.Count > 0)
                {
                    trans.GenerateControl(OpCodes.Dup);
                }
                CallTranslate((dynamic)element.CallScope, element.Arguments, trans);
            }
        }

        private void CallTranslate(RoutineSymbol call, TupleList arguments, Translator trans)
        {
            Translate((dynamic)arguments, trans);
            trans.GenerateCall(call);
        }

        private void CallTranslate(ClassSymbol call, TupleList arguments, Translator trans)
        {
            Translate((dynamic)arguments, trans);
            trans.GenerateCall(call);
        }

        private void CallTranslate(VariantSymbol call, TupleList arguments, Translator trans)
        {
            Translate((dynamic)arguments, trans);
            if (arguments.Count > 0)
            {
                trans.GenerateStore(call);
            }
            trans.GenerateLoad(call);
        }

        private void CallTranslate(ThisSymbol call, TupleList arguments, Translator trans)
        {
            Translate((dynamic)arguments, trans);
            if (arguments.Count > 0)
            {
                trans.GenerateStore(call);
            }
            trans.GenerateLoad(call);
        }

        private void CallTranslate(BooleanSymbol call, TupleList arguments, Translator trans)
        {
            trans.GeneratePrimitive(call.Value);
        }

        private void CallTranslate(CalculatePragma call, TupleList arguments, Translator trans)
        {
            ChildTranslate(arguments, trans);
            switch (call.CalculateType)
            {
                case CalculatePragmaType.Add: trans.GenerateControl(OpCodes.Add); break;
                case CalculatePragmaType.Sub: trans.GenerateControl(OpCodes.Sub); break;
                case CalculatePragmaType.Mul: trans.GenerateControl(OpCodes.Mul); break;
                case CalculatePragmaType.Div: trans.GenerateControl(OpCodes.Div); break;
                case CalculatePragmaType.Mod: trans.GenerateControl(OpCodes.Rem); break;
                case CalculatePragmaType.EQ: trans.GenerateControl(OpCodes.Ceq); break;
                case CalculatePragmaType.NE: trans.GenerateControl(OpCodes.Ceq); trans.GenerateControl(OpCodes.Ldc_I4_1); trans.GenerateControl(OpCodes.Xor); break;
                case CalculatePragmaType.LT: trans.GenerateControl(OpCodes.Clt); break;
                case CalculatePragmaType.LE: trans.GenerateControl(OpCodes.Cgt); trans.GenerateControl(OpCodes.Ldc_I4_1); trans.GenerateControl(OpCodes.Xor); break;
                case CalculatePragmaType.GT: trans.GenerateControl(OpCodes.Cgt); break;
                case CalculatePragmaType.GE: trans.GenerateControl(OpCodes.Clt); trans.GenerateControl(OpCodes.Ldc_I4_1); trans.GenerateControl(OpCodes.Xor); break;
                default: throw new ArgumentException();
            }
        }

        private void CallTranslate(CastPragma call, TupleList arguments, Translator trans)
        {
            Translate((dynamic)arguments[1], trans);
            var cls = arguments[0].ReturnType as ClassSymbol;
            var prim = cls.PrimitiveType;
            switch(prim)
            {
                case PrimitivePragmaType.Integer8: trans.GenerateControl(OpCodes.Conv_I1); break;
                case PrimitivePragmaType.Integer16: trans.GenerateControl(OpCodes.Conv_I2); break;
                case PrimitivePragmaType.Integer32: trans.GenerateControl(OpCodes.Conv_I4); break;
                case PrimitivePragmaType.Integer64: trans.GenerateControl(OpCodes.Conv_I8); break;
                case PrimitivePragmaType.Natural8: trans.GenerateControl(OpCodes.Conv_U1); break;
                case PrimitivePragmaType.Natural16: trans.GenerateControl(OpCodes.Conv_U2); break;
                case PrimitivePragmaType.Natural32: trans.GenerateControl(OpCodes.Conv_U4); break;
                case PrimitivePragmaType.Natural64: trans.GenerateControl(OpCodes.Conv_U8); break;
                case PrimitivePragmaType.Binary32: trans.GenerateControl(OpCodes.Conv_R4); break;
                case PrimitivePragmaType.Binary64: trans.GenerateControl(OpCodes.Conv_R8); break;
                default: throw new ArgumentException();
            }
        }

        private bool TranslateAccess(Element element, Translator trans)
        {
            return false;
        }

        private bool TranslateAccess(IdentifierAccess element, Translator trans)
        {
            if (element.IsTacitThis)
            {
                trans.GenerateLoad(element.ThisReference);
                return true;
            }
            return false;
        }

        private bool TranslateAccess(MemberAccess element, Translator trans)
        {
            Translate((dynamic)element.Access, trans);
            return true;
        }

        private void TranslateBrunch(Element element, Translator trans, Label bfalse, bool isnot)
        {
            Translate((dynamic)element, trans);
            if (isnot)
            {
                trans.GenerateJump(OpCodes.Brfalse, bfalse);
            }
            else
            {
                trans.GenerateJump(OpCodes.Brtrue, bfalse);
            }
        }

        private void TranslateBrunch(UnStatement element, Translator trans, Label bfalse, bool isnot)
        {
            TranslateBrunch((dynamic)element.Exp, trans, bfalse, !isnot);
        }

        private void TranslateBrunch(Condition element, Translator trans, Label bfalse, bool isnot)
        {
            Translate((dynamic)element.Left, trans);
            if (element.IsConnection)
            {
                Translate((dynamic)element.VirtualRight, trans);
                TranslateBrunchCall(element.CallScope, trans, bfalse, isnot);
                TranslateBrunch((dynamic)element.Right, trans, bfalse, isnot);
            }
            else
            {
                Translate((dynamic)element.Right, trans);
                TranslateBrunchCall(element.CallScope, trans, bfalse, isnot);
            }
        }

        private void TranslateBrunch(Logical element, Translator trans, Label bfalse, bool isnot)
        {
            if(element.Operator == TokenType.Or)
            {
                var bend = trans.CreateLabel();
                TranslateBrunch((dynamic)element.Left, trans, bend, !isnot);
                TranslateBrunch((dynamic)element.Right, trans, bfalse, isnot);
                trans.MarkLabel(bend);
            }
            else if (element.Operator == TokenType.And)
            {
                TranslateBrunch((dynamic)element.Left, trans, bfalse, isnot);
                TranslateBrunch((dynamic)element.Right, trans, bfalse, isnot);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void TranslateBrunchCall(Scope call, Translator trans, Label bfalse, bool isnot)
        {
            //todo 短縮コードの出力に対応する。
        }
    }
}
