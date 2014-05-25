using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Expression;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CliTranslate
{
    public class TranslateManager
    {
        private SortedSet<Scope> SpreadQueue;
        private Dictionary<Scope, Translator> TransDictionary;
        private RootTranslator Root;

        public TranslateManager(string name, string dir = null)
        {
            SpreadQueue = new SortedSet<Scope>(new SpreadPriority());
            TransDictionary = new Dictionary<Scope, Translator>();
            Root = new RootTranslator(name, dir);
        }

        class SpreadPriority : IComparer<Scope>
        {
            public int Compare(Scope x, Scope y)
            {
                var a = ScopePriority(x) - ScopePriority(y);
                if(a != 0)
                {
                    return a;
                }
                return x.Id - y.Id;
            }

            private int ScopePriority(Scope scope)
            {
                if (scope is Root) return 0;
                if (scope is NameSpace) return 1;
                if (scope is DeclateModule) return 2;
                if (scope is DeclateClass) return 3;
                if (scope is DeclateEnum) return 4;
                if (scope is DeclateGeneric) return 5;
                if (scope is DeclateRoutine) return 10;
                if (scope is DeclateOperator) return 11;
                if (scope is DeclateArgument) return 20;
                if (scope is DeclateVariant) return 21;
                return 100;
            }
        }

        public void TranslateTo(Root root, ImportManager manager)
        {
            manager.TranslateImport(Root);
            TransDictionary.Add(root, Root);
            ChildSpreadTranslate(root, Root);
            EnumSpreadTranslate();
            Translate(root, Root);
            Root.BuildCode();
        }

        public void Save()
        {
            Root.Save();
        }

        public void Run()
        {
            Root.Run();
        }

        private void EnumSpreadTranslate()
        {
            while(SpreadQueue.Count > 0)
            {
                var v = SpreadQueue.Min;
                SpreadQueue.Remove(v);
                var trans = TransDictionary[v.CurrentScope];
                SpreadTranslate((dynamic)v, trans);
            }
        }

        private void ChildSpreadTranslate(Scope scope, Translator trans)
        {
            foreach (var v in scope.ScopeChild)
            {
                if (v == null || v.IsImport || v is IPragma)
                {
                    continue;
                }
                SpreadQueue.Add(v);
            }
        }

        private void SpreadTranslate(Scope scope, Translator trans)
        {
            ChildSpreadTranslate(scope, trans);
        }

        private void SpreadTranslate(DeclateModule scope, Translator trans)
        {
            var temp = trans.CreateModule(scope);
            TransDictionary.Add(scope, temp);
            ChildSpreadTranslate(scope, temp);
        }

        private void SpreadTranslate(DeclateClass scope, Translator trans)
        {
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
            List<Scope> argumentType = new List<Scope>();
            foreach(var v in scope.ArgumentType)
            {
                argumentType.Add(v);
            }
            RoutineTranslator temp;
            if (scope.IsConstructor)
            {
                var cls = (ClassTranslator)trans;
                temp = cls.CreateConstructor(scope, argumentType.ToArray());
            }
            else if (scope.IsDestructor)
            {
                var cls = (ClassTranslator)trans;
                temp = cls.CreateDestructor(scope);
            }
            else
            {
                temp = trans.CreateRoutine(scope, scope.ReturnType, argumentType.ToArray());
            }
            TransDictionary.Add(scope, temp);
            var arguments = new List<Scope>();
            foreach (var v in scope.Arguments)
            {
                arguments.Add((Scope)v);
            }
            temp.CreateArguments(arguments.ToArray());
            ChildSpreadTranslate(scope, temp);
        }

        private void SpreadTranslate(DeclateVariant scope, Translator trans)
        {
            trans.CreateVariant(scope, scope.DataType);
        }

        private void SpreadTranslate(DeclateArgument scope, Translator trans)
        {
            return;
        }

        private void SpreadTranslate(DeclateGeneric scope, Translator trans)
        {
            return;
        }

        private void ChildTranslate(Element element, Translator trans)
        {
            foreach(var v in element)
            {
                if (v == null || v.IsImport || v is IPragma)
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
            if(element.Parent is NameSpace && !(element.Parent is DeclateModule))
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
            var rout = element.Parent as DeclateRoutine;
            if (rout != null && rout.IsThisReturn)
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
            PrimitivePragmaType prim = arguments[0].DataType.GetPrimitiveType();
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
