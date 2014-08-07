using AbstractSyntax;
using AbstractSyntax.Declaration;
using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
using AbstractSyntax.Pragma;
using AbstractSyntax.Statement;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

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

        private void ChildSpreadTranslate(Element element)
        {
            foreach (var v in element)
            {
                var scope = v as Scope;
                if (scope != null)
                {
                    RelaySpreadTranslate(scope);
                }
                ChildSpreadTranslate(v);
            }
        }

        private void RelaySpreadTranslate(Scope scope)
        {
            if (TransDictionary.ContainsKey(scope))
            {
                return;
            }
            RelaySpreadTranslate(scope.CurrentScope);
            SpreadTranslate((dynamic)scope, TransDictionary[scope.CurrentScope]);
        }

        private void SpreadTranslate(Scope scope, Translator trans)
        {
            TransDictionary.Add(scope, trans);
        }

        private void SpreadTranslate(ModuleDeclaration scope, Translator trans)
        {
            var temp = trans.CreateModule(scope);
            TransDictionary.Add(scope, temp);
        }

        private void SpreadTranslate(ClassDeclaration scope, Translator trans)
        {
            foreach (var v in scope.Inherit)
            {
                RelaySpreadTranslate(v);
            }
            if (false)
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

        private void SpreadTranslate(RoutineDeclaration scope, Translator trans)
        {
            RoutineTranslator temp;
            if (scope.IsConstructor)
            {
                foreach (var v in scope.ArgumentTypes)
                {
                    RelaySpreadTranslate(v);
                }
                var cls = (ClassTranslator)trans;
                temp = cls.CreateConstructor(scope);
                TransDictionary.Add(scope, temp);
                temp.CreateArguments(scope.DecArguments.Cast<ArgumentDeclaration>());
                return;
            }
            if (scope.IsDestructor)
            {
                var cls = (ClassTranslator)trans;
                temp = cls.CreateDestructor(scope);
                TransDictionary.Add(scope, temp);
                return;
            }
            temp = trans.CreateRoutine(scope);
            TransDictionary.Add(scope, temp);
            temp.CreateGenerics(scope.Generics);
            RelaySpreadTranslate(scope.CallReturnType);
            foreach (var v in scope.ArgumentTypes)
            {
                RelaySpreadTranslate(v);
            }
            temp.CreateReturn(scope.CallReturnType);
            temp.CreateArguments(scope.DecArguments.Cast<ArgumentDeclaration>(), scope.ArgumentTypes);
        }

        private void SpreadTranslate(VariantDeclaration scope, Translator trans)
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
            foreach (var v in element)
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

        private void Translate(ModuleDeclaration element, Translator trans)
        {
            var temp = TransDictionary[element];
            ChildTranslate(element, temp);
        }

        private void Translate(ClassDeclaration element, Translator trans)
        {
            var temp = TransDictionary[element];
            Translate((dynamic)element.Block, temp);
        }

        private void Translate(RoutineDeclaration element, Translator trans)
        {
            var temp = TransDictionary[element];
            Translate((dynamic)element.Block, temp);
            if (element.IsDefaultThisReturn)
            {
                temp.GenerateControl(OpCodes.Ldarg_0);
                temp.GenerateControl(OpCodes.Ret);
            }
        }

        private void Translate(VariantDeclaration element, Translator trans)
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
                Translate(element.Then, bt);
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
                Translate(element.Then, bt);
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

        private void Translate(NumericLiteral element, Translator trans)
        {
            dynamic number = element.Parse();
            trans.GeneratePrimitive(number);
        }

        private void Translate(StringLiteral element, Translator trans)
        {
            if (element.Texts.Count == 0)
            {
                trans.GeneratePrimitive(string.Empty);
                return;
            }
            var c = element.Texts.Count;
            for (var i = 0; i < c; ++i)
            {
                Translate((dynamic)element.Texts[i], trans);
                trans.GenerateToString(element.Texts[i].ReturnType);
                if (i > 0)
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
            CallTranslate((dynamic)element.CallScope, trans);
        }

        private void Translate(Compare element, Translator trans)
        {
            Translate((dynamic)element.Left, trans);
            if (element.IsRightConnection)
            {
                Translate((dynamic)element.VirtualRight, trans);
                CallTranslate((dynamic)element.CallScope, trans);
                Translate((dynamic)element.Right, trans);
                trans.GenerateControl(OpCodes.And);
            }
            else
            {
                Translate((dynamic)element.Right, trans);
                CallTranslate((dynamic)element.CallScope, trans);
            }
        }

        private void Translate(Logical element, Translator trans)
        {
            var bend = trans.CreateLabel();
            if (element.Operator == TokenType.Or)
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
            Translate((dynamic)element.Exp, trans);
        }

        private void Translate(Postfix element, Translator trans)
        {
            return; //todo 参照や型情報を返すようにする。
        }

        private void Translate(Identifier element, Translator trans)
        {
            if (element.IsTacitThis)
            {
                trans.GenerateLoad(element.ThisReference);
            }
            CallTranslate((dynamic)element.CallScope, trans);
        }

        private void Translate(MemberAccess element, Translator trans)
        {
            Translate((dynamic)element.Access, trans);
            CallTranslate((dynamic)element.CallScope, trans);
        }

        private void Translate(CallRoutine element, Translator trans)
        {
            if (element.IsCalculate)
            {
                var acs = TranslateAccess((dynamic)element.Access, trans);
                if (acs && element.CallScope is PropertyPragma && element.Arguments.Count > 0)
                {
                    trans.GenerateControl(OpCodes.Dup);
                }
                Translate((dynamic)element.Left, trans);
                Translate((dynamic)element.Right, trans);
                CallTranslate((dynamic)element.CalculateCallScope, trans);
                CallTranslate((dynamic)element.CallScope, trans);
            }
            else
            {
                var acs = TranslateAccess((dynamic)element.Access, trans);
                if (acs && element.CallScope is PropertyPragma && element.Arguments.Count > 0)
                {
                    trans.GenerateControl(OpCodes.Dup);
                }
                Translate((dynamic)element.Arguments, trans);
                CallTranslate((dynamic)element.CallScope, trans);
            }
        }

        private void CallTranslate(RoutineSymbol call, Translator trans)
        {
            trans.GenerateCall(call);
        }

        private void CallTranslate(BooleanSymbol call, Translator trans)
        {
            trans.GeneratePrimitive(call.Value);
        }

        private void CallTranslate(PropertyPragma call, Translator trans)
        {
            if(call.IsSet)
            {
                trans.GenerateStore(call.Variant);
            }
            trans.GenerateLoad(call.Variant);
        }

        private void CallTranslate(DyadicOperatorPragma call, Translator trans)
        {
            switch (call.CalculateType)
            {
                case TokenType.Add: trans.GenerateControl(OpCodes.Add); break;
                case TokenType.Subtract: trans.GenerateControl(OpCodes.Sub); break;
                case TokenType.Multiply: trans.GenerateControl(OpCodes.Mul); break;
                case TokenType.Divide: trans.GenerateControl(OpCodes.Div); break;
                case TokenType.Modulo: trans.GenerateControl(OpCodes.Rem); break;
                case TokenType.Equal: trans.GenerateControl(OpCodes.Ceq); break;
                case TokenType.NotEqual: trans.GenerateControl(OpCodes.Ceq); trans.GenerateControl(OpCodes.Ldc_I4_1); trans.GenerateControl(OpCodes.Xor); break;
                case TokenType.LessThan: trans.GenerateControl(OpCodes.Clt); break;
                case TokenType.LessThanOrEqual: trans.GenerateControl(OpCodes.Cgt); trans.GenerateControl(OpCodes.Ldc_I4_1); trans.GenerateControl(OpCodes.Xor); break;
                case TokenType.GreaterThan: trans.GenerateControl(OpCodes.Cgt); break;
                case TokenType.GreaterThanOrEqual: trans.GenerateControl(OpCodes.Clt); trans.GenerateControl(OpCodes.Ldc_I4_1); trans.GenerateControl(OpCodes.Xor); break;
                default: throw new ArgumentException();
            }
        }

        private void CallTranslate(CastPragma call, Translator trans)
        {
            var prim = call.PrimitiveType;
            switch (prim)
            {
                case CastPragmaType.Integer8: trans.GenerateControl(OpCodes.Conv_I1); break;
                case CastPragmaType.Integer16: trans.GenerateControl(OpCodes.Conv_I2); break;
                case CastPragmaType.Integer32: trans.GenerateControl(OpCodes.Conv_I4); break;
                case CastPragmaType.Integer64: trans.GenerateControl(OpCodes.Conv_I8); break;
                case CastPragmaType.Natural8: trans.GenerateControl(OpCodes.Conv_U1); break;
                case CastPragmaType.Natural16: trans.GenerateControl(OpCodes.Conv_U2); break;
                case CastPragmaType.Natural32: trans.GenerateControl(OpCodes.Conv_U4); break;
                case CastPragmaType.Natural64: trans.GenerateControl(OpCodes.Conv_U8); break;
                case CastPragmaType.Binary32: trans.GenerateControl(OpCodes.Conv_R4); break;
                case CastPragmaType.Binary64: trans.GenerateControl(OpCodes.Conv_R8); break;
                default: throw new ArgumentException();
            }
        }

        private bool TranslateAccess(Element element, Translator trans)
        {
            return false;
        }

        private bool TranslateAccess(Identifier element, Translator trans)
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

        private void TranslateBrunch(Compare element, Translator trans, Label bfalse, bool isnot)
        {
            Translate((dynamic)element.Left, trans);
            if (element.IsRightConnection)
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
            if (element.Operator == TokenType.Or)
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
