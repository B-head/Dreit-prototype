using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;

namespace CliTranslate
{
    public class TranslateManager
    {
        private SortedSet<Scope> SpreadQueue;
        private Dictionary<Scope, Translator> TransDictionary;
        private RootTranslator Root;

        public TranslateManager(string name)
        {
            SpreadQueue = new SortedSet<Scope>(new SpreadPriority());
            TransDictionary = new Dictionary<Scope, Translator>();
            Root = new RootTranslator(name);
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
                if (scope is DeclateGeneric) return 9;
                if (scope is DeclateRoutine) return 10;
                if (scope is DeclateOperator) return 11;
                if (scope is DeclateArgument) return 20;
                if (scope is DeclateVariant) return 21;
                if (scope is VoidSymbol) return 30;
                if (scope is ThisSymbol) return 31;
                if (scope is AliasDirective) return 32;
                throw new ArgumentException();
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
            var temp = trans.CreateRoutine(scope, scope.ReturnType, argumentType.ToArray());
            TransDictionary.Add(scope, temp);
            var argument = new List<Scope>();
            foreach (var v in scope.Argument)
            {
                argument.Add((Scope)v);
            }
            temp.CreateArguments(argument.ToArray());
            ChildSpreadTranslate(scope, temp);
        }

        private void SpreadTranslate(DeclateVariant scope, Translator trans)
        {
            trans.CreateVariant(scope, scope.DataType);
        }

        private void SpreadTranslate(DeclateArgument scope, Translator trans)
        {
            //何もしない。
        }

        private void SpreadTranslate(DeclateGeneric scope, Translator trans)
        {
            //何もしない。
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
                    trans.GenerateControl(CodeType.Pop);
                }
            }
            if (element.Count <= 0 || !(element[element.Count - 1] is ReturnDirective)) //todo VoidSymbolで調べた方がいい？
            {
                trans.GenerateControl(CodeType.Ret);
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

        private void TranslateAddress(DeclateVariant element, Translator trans)
        {
            TranslateAddress((dynamic)element.Ident, trans);
        }

        private void TranslateAssign(DeclateVariant element, Translator trans)
        {
            TranslateAssign((dynamic)element.Ident, trans);
        }

        private void Translate(IdentifierAccess element, Translator trans)
        {
            if (element.ScopeReference is ThisSymbol)
            {
                trans.GenerateLoad(element.ThisReference);
            }
            else if (element.IsTacitThis)
            {
                trans.GenerateLoad(element.ThisReference);
                trans.GenerateLoad(element.ScopeReference);
            }
            else
            {
                trans.GenerateLoad(element.ScopeReference);
            }
        }

        private void TranslateAddress(IdentifierAccess element, Translator trans)
        {
            if (element.ScopeReference is ThisSymbol)
            {
                trans.GenerateLoad(element.ThisReference, true);
            }
            else if (element.IsTacitThis)
            {
                trans.GenerateLoad(element.ThisReference, true);
                trans.GenerateLoad(element.ScopeReference, true);
            }
            else
            {
                trans.GenerateLoad(element.ScopeReference, true);
            }
        }

        private void TranslateAssign(IdentifierAccess element, Translator trans)
        {
            if (element.ScopeReference is ThisSymbol)
            {
                trans.GenerateStore(element.ThisReference);
            }
            else
            {
                trans.GenerateStore(element.ScopeReference);
            }
        }

        private Element TranslateAccess(IdentifierAccess element, Translator trans)
        {
            if (element.IsTacitThis)
            {
                trans.GenerateLoad(element.ThisReference);
            }
            return element;
        }

        private Element TranslateAccess(MemberAccess element, Translator trans)
        {
            Translate((dynamic)element.Left, trans);
            var temp = element.Right as MemberAccess;
            if (temp != null)
            {
                return TranslateAccess(temp, trans);
            }
            return element.Right;
        }

        private Element TranslateAccess(Element element, Translator trans)
        {
            return element;
        }

        private void Translate(DyadicCalculate element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateCall(element.CallScope);
        }

        private void Translate(LeftAssign element, Translator trans)
        {
            var refer = TranslateAccess((dynamic)element.Left, trans);
            if (element.ConversionRoutine is UndefinedSymbol)
            {
                Translate((dynamic)element.Right, trans);
                TranslateAssign((dynamic)refer, trans);
                Translate((dynamic)refer, trans);
            }
            else
            {
                Translate((dynamic)refer, trans);
                Translate((dynamic)element.Right, trans);
                trans.GenerateCall(element.ConversionRoutine);

                TranslateAssign((dynamic)refer, trans); //todo 後でこの処理を無くす。
                Translate((dynamic)refer, trans);
            }
        }

        private void Translate(RightAssign element, Translator trans)
        {
            var refer = TranslateAccess((dynamic)element.Right, trans);
            if (element.ConversionRoutine is UndefinedSymbol)
            {
                Translate((dynamic)element.Left, trans);
                TranslateAssign((dynamic)refer, trans);
                Translate((dynamic)refer, trans);
            }
            else
            {
                Translate((dynamic)refer, trans);
                Translate((dynamic)element.Left, trans);
                trans.GenerateCall(element.ConversionRoutine);

                TranslateAssign((dynamic)refer, trans); //todo 後でこの処理を無くす。
                Translate((dynamic)refer, trans);
            }
        }

        private void Translate(CallRoutine element, Translator trans)
        {
            CallTranslate((dynamic)element.CallScope, element, trans);
        }

        private void CallTranslate(Scope call, CallRoutine element, Translator trans)
        {
            TranslateAccess((dynamic)element.Access, trans);
            Translate((dynamic)element.Argument, trans);
            trans.GenerateCall(call);
        }

        private void CallTranslate(CalculatePragma call, CallRoutine element, Translator trans)
        {
            foreach (var v in element.Argument)
            {
                Translate((dynamic)v, trans);
            }
            switch (call.Type)
            {
                case CalculatePragmaType.Add: trans.GenerateControl(CodeType.Add); break;
                case CalculatePragmaType.Sub: trans.GenerateControl(CodeType.Sub); break;
                case CalculatePragmaType.Mul: trans.GenerateControl(CodeType.Mul); break;
                case CalculatePragmaType.Div: trans.GenerateControl(CodeType.Div); break;
                case CalculatePragmaType.Mod: trans.GenerateControl(CodeType.Mod); break;
                default: throw new ArgumentException();
            }
        }

        private void CallTranslate(CastPragma call, CallRoutine element, Translator trans)
        {
            Translate((dynamic)element.Argument[1], trans);
            PrimitivePragmaType prim = element.ArgumentType[0].GetPrimitiveType();
            switch(prim)
            {
                case PrimitivePragmaType.Integer8: trans.GenerateControl(CodeType.ConvI1); break;
                case PrimitivePragmaType.Integer16: trans.GenerateControl(CodeType.ConvI2); break;
                case PrimitivePragmaType.Integer32: trans.GenerateControl(CodeType.ConvI4); break;
                case PrimitivePragmaType.Integer64: trans.GenerateControl(CodeType.ConvI8); break;
                case PrimitivePragmaType.Natural8: trans.GenerateControl(CodeType.ConvU1); break;
                case PrimitivePragmaType.Natural16: trans.GenerateControl(CodeType.ConvU2); break;
                case PrimitivePragmaType.Natural32: trans.GenerateControl(CodeType.ConvU4); break;
                case PrimitivePragmaType.Natural64: trans.GenerateControl(CodeType.ConvU8); break;
                case PrimitivePragmaType.Binary32: trans.GenerateControl(CodeType.ConvR4); break;
                case PrimitivePragmaType.Binary64: trans.GenerateControl(CodeType.ConvR8); break;
                default: throw new ArgumentException();
            }
            TranslateAssign((dynamic)element.Argument[0], trans);
            Translate((dynamic)element.Argument[0], trans);
        }

        private void Translate(NumberLiteral element, Translator trans)
        {
            dynamic number = element.Parse();
            trans.GeneratePrimitive(number);
        }

        private void Translate(ReturnDirective element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateControl(CodeType.Ret);
        }

        private void Translate(EchoDirective element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateEcho(element.Exp.DataType);
        }
    }
}
