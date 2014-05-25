using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;

namespace CliTranslate
{
    public class TranslateManager
    {
        private Dictionary<IScope, Translator> TransDictionary;
        private LinkedList<IScope> SpreadQueue;
        private RootTranslator Root;

        public TranslateManager(string name, string dir = null)
        {
            TransDictionary = new Dictionary<IScope, Translator>();
            SpreadQueue = new LinkedList<IScope>();
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
            SpreadTranslate(root, Root);
            while (SpreadQueue.Count > 0)
            {
                var f = SpreadQueue.First.Value;
                SpreadQueue.RemoveFirst();
                Translator t = null;
                if (TransDictionary.ContainsKey(f.CurrentIScope))
                {
                    t = TransDictionary[f.CurrentIScope];
                }
                SpreadTranslate((dynamic)f, t);
            }
            Translate(root, Root);
            Root.BuildCode();
        }

        private void ChildSpreadTranslate(IScope scope, Translator trans)
        {
            foreach (var v in scope.ScopeChild)
            {
                if (v == null)
                {
                    continue;
                }
                if(v is IDataType)
                {
                    SpreadQueue.AddFirst(v);
                }
                else
                {
                    SpreadQueue.AddLast(v);
                }
            }
        }

        private void SpreadTranslate(IScope scope, Translator trans)
        {
            ChildSpreadTranslate(scope, trans);
        }

        private void SpreadTranslate(DeclateModule scope, Translator trans)
        {
            if(TransDictionary.ContainsKey(scope))
            {
                return;
            }
            var temp = trans.CreateModule(scope);
            TransDictionary.Add(scope, temp);
            ChildSpreadTranslate(scope, temp);
        }

        private void SpreadTranslate(DeclateClass scope, Translator trans)
        {
            if (TransDictionary.ContainsKey(scope))
            {
                return;
            }
            PrimitivePragmaType prim = scope.GetPrimitiveType();
            if (prim != PrimitivePragmaType.NotPrimitive)
            {
                var temp = trans.CreatePrimitive(scope, prim);
                TransDictionary.Add(scope, temp);
                ChildSpreadTranslate(scope, temp);
            }
            else
            {
                var temp = trans.CreateClass(scope);
                TransDictionary.Add(scope, temp);
                ChildSpreadTranslate(scope, temp);
            }
        }

        private void SpreadTranslate(DeclateRoutine scope, Translator trans)
        {
            if (TransDictionary.ContainsKey(scope))
            {
                return;
            }
            RoutineTranslator temp;
            if (scope.IsConstructor)
            {
                var cls = (ClassTranslator)trans;
                temp = cls.CreateConstructor(scope, scope.ArgumentType);
            }
            else if (scope.IsDestructor)
            {
                var cls = (ClassTranslator)trans;
                temp = cls.CreateDestructor(scope);
            }
            else
            {
                temp = trans.CreateRoutine(scope, scope.ReturnType, scope.ArgumentType);
            }
            TransDictionary.Add(scope, temp);
            temp.CreateArguments(scope.Arguments.Cast<IScope>());
            ChildSpreadTranslate(scope, temp);
        }

        private void SpreadTranslate(DeclateVariant scope, Translator trans)
        {
            if (TransDictionary.ContainsKey(scope))
            {
                return;
            }
            trans.CreateVariant(scope, scope.DataType);
        }

        private void ChildTranslate(IElement element, Translator trans)
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

        private void Translate(IElement element, Translator trans)
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
            foreach (Element v in element)
            {
                Translate((dynamic)v, trans);
                if (!v.IsVoidValue && !element.IsInline)
                {
                    trans.GenerateControl(OpCodes.Pop);
                }
            }
            if (element.IsThisReturn)
            {
                trans.GenerateControl(OpCodes.Ldarg_0);
                trans.GenerateControl(OpCodes.Ret);
            }
            else if (element.Count <= 0 || !(element[element.Count - 1] is ReturnDirective)) //todo VoidSymbolで調べた方がいい？
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
        }

        private void Translate(DeclateVariant element, Translator trans)
        {
            Translate((dynamic)element.Ident, trans);
        }

        private void Translate(ReturnDirective element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateControl(OpCodes.Ret);
        }

        private void Translate(EchoDirective element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateEcho(element.Exp.DataType);
        }

        private void Translate(NumberLiteral element, Translator trans)
        {
            dynamic number = element.Parse();
            trans.GeneratePrimitive(number);
        }

        private void Translate(Calculate element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateCall(element.CallScope);
        }

        private void Translate(IdentifierAccess element, Translator trans)
        {
            TranslateAccess((dynamic)element, trans);
            CallTranslate((dynamic)element.CallScope, new TupleList(), trans);
        }

        private void Translate(LeftAssign element, Translator trans)
        {
            var acs = TranslateAccess((dynamic)element.Left, trans);
            if (acs && element.CallScope is VariantSymbol && element.Arguments.Count > 0)
            {
                trans.GenerateControl(OpCodes.Dup);
            }
            CallTranslate((dynamic)element.CallScope, element.Arguments, trans);
        }

        private void Translate(RightAssign element, Translator trans)
        {
            var acs = TranslateAccess((dynamic)element.Right, trans);
            if (acs && element.CallScope is VariantSymbol && element.Arguments.Count > 0)
            {
                trans.GenerateControl(OpCodes.Dup);
            }
            CallTranslate((dynamic)element.CallScope, element.Arguments, trans);
        }

        private void Translate(CallRoutine element, Translator trans)
        {
            var acs = TranslateAccess((dynamic)element.Access, trans);
            if (acs && element.CallScope is VariantSymbol && element.Arguments.Count > 0)
            {
                trans.GenerateControl(OpCodes.Dup);
            }
            CallTranslate((dynamic)element.CallScope, element.Arguments, trans);
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

        private void CallTranslate(CalculatePragma call, TupleList arguments, Translator trans)
        {
            foreach (var v in arguments)
            {
                Translate((dynamic)v, trans);
            }
            switch (call.Type)
            {
                case CalculatePragmaType.Add: trans.GenerateControl(OpCodes.Add); break;
                case CalculatePragmaType.Sub: trans.GenerateControl(OpCodes.Sub); break;
                case CalculatePragmaType.Mul: trans.GenerateControl(OpCodes.Mul); break;
                case CalculatePragmaType.Div: trans.GenerateControl(OpCodes.Div); break;
                case CalculatePragmaType.Mod: trans.GenerateControl(OpCodes.Rem); break;
                default: throw new ArgumentException();
            }
        }

        private void CallTranslate(CastPragma call, TupleList arguments, Translator trans)
        {
            Translate((dynamic)arguments[1], trans);
            var cls = arguments[0].DataType as ClassSymbol;
            var prim = cls.GetPrimitiveType();
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
            Translate((dynamic)element.Left, trans);
            var temp = element.Right as MemberAccess;
            if (temp != null)
            {
                TranslateAccess(temp, trans);
            }
            return true;
        }

        private bool TranslateAccess(Element element, Translator trans)
        {
            return false;
        }
    }
}
