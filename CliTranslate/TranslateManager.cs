using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

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
                if (scope is NameSpace) return 2;
                if (scope is DeclateModule) return 2;
                if (scope is DeclateClass) return 3;
                if (scope is DeclateEnum) return 4;
                if (scope is DeclateGeneric) return 9;
                if (scope is DeclateRoutine) return 10;
                if (scope is DeclateOperator) return 11;
                if (scope is DeclateArgument) return 20;
                if (scope is DeclateVariant) return 21;
                if (scope is VoidScope) return 30;
                if (scope is ThisScope) return 31;
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
            Root.Save();
        }

        private void EnumSpreadTranslate()
        {
            while(SpreadQueue.Count > 0)
            {
                var v = SpreadQueue.Min;
                SpreadQueue.Remove(v);
                var trans = TransDictionary[v.ScopeParent];
                SpreadTranslate((dynamic)v, trans);
            }
        }

        private void ChildSpreadTranslate(Scope scope, Translator trans)
        {
            foreach (var v in scope.ScopeChild.Values)
            {
                if (v == null || v.IsImport || v.IsPragma)
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
            var temp = trans.CreateModule(scope.FullPath);
            TransDictionary.Add(scope, temp);
            ChildSpreadTranslate(scope, temp);
        }

        private void SpreadTranslate(DeclateClass scope, Translator trans)
        {
            PrimitivePragma prim = null;
            if (scope.InheritRefer.Count == 1)
            {
                prim = scope.InheritRefer[0] as PrimitivePragma;
            }
            if (prim != null)
            {
                var temp = trans.CreatePrimitive(scope.FullPath, prim.Type);
                TransDictionary.Add(scope, temp);
                ChildSpreadTranslate(scope, temp);
            }
            else
            {
                var temp = trans.CreateClass(scope.FullPath);
                TransDictionary.Add(scope, temp);
                ChildSpreadTranslate(scope, temp);
            }
        }

        private void SpreadTranslate(DeclateRoutine scope, Translator trans)
        {
            List<FullPath> argumentType = new List<FullPath>();
            foreach(var v in scope.ArgumentType)
            {
                argumentType.Add(v.FullPath);
            }
            var temp = trans.CreateRoutine(scope.FullPath, scope.ReturnType.FullPath, argumentType.ToArray());
            TransDictionary.Add(scope, temp);
            var argument = new List<FullPath>();
            foreach (var v in scope.Argument)
            {
                argument.Add(((Scope)v).FullPath);
            }
            temp.CreateArguments(argument.ToArray());
            ChildSpreadTranslate(scope, temp);
        }

        private void SpreadTranslate(DeclateVariant scope, Translator trans)
        {
            trans.CreateVariant(scope.FullPath, scope.DataType.FullPath);
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
                if (v == null || v.IsImport || v.IsPragma)
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
            foreach (Element v in element)
            {
                Translate((dynamic)v, trans);
                if (!v.IsVoidValue && !element.IsInline)
                {
                    trans.GenerateControl(CodeType.Pop);
                }
            }
            if (element.Count <= 0 || !(element.GetChild(element.Count - 1) is ReturnDirective))
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

        private void TranslateAssign(DeclateVariant element, Translator trans)
        {
            TranslateAssign((dynamic)element.Ident, trans);
        }

        private void Translate(IdentifierAccess element, Translator trans)
        {
            if(element.Refer is ThisScope)
            {
                trans.GenerateControl(CodeType.This);
            }
            else if (element.IsTacitThis)
            {
                trans.GenerateControl(CodeType.This);
                trans.GenerateLoad(element.Refer.FullPath);
            }
            else
            {
                trans.GenerateLoad(element.Refer.FullPath);
            }
        }

        private void TranslateAssign(IdentifierAccess element, Translator trans)
        {
            trans.GenerateStore(element.Refer.FullPath);
        }

        private Element TranslateAccess(IdentifierAccess element, Translator trans)
        {
            if (element.IsTacitThis)
            {
                trans.GenerateControl(CodeType.This);
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
            trans.GenerateCall(element.ReferOp.FullPath);
        }

        private void Translate(LeftAssign element, Translator trans)
        {
            var refer = TranslateAccess((dynamic)element.Left, trans);
            Translate((dynamic)element.Right, trans);
            TranslateAssign((dynamic)refer, trans);
            Translate((dynamic)refer, trans);
        }

        private void Translate(RightAssign element, Translator trans)
        {
            var refer = TranslateAccess((dynamic)element.Right, trans);
            Translate((dynamic)element.Left, trans);
            TranslateAssign((dynamic)refer, trans);
            Translate((dynamic)refer, trans);
        }

        private void Translate(CallRoutine element, Translator trans)
        {
            var pragma = element.Access.Reference as CalculatePragma;
            if(pragma != null)
            {
                PragmaTranslate(pragma, element.Argument, trans);
                return;
            }
            TranslateAccess((dynamic)element.Access, trans);
            Translate((dynamic)element.Argument, trans);
            trans.GenerateCall(element.Access.Reference.FullPath);
        }

        private void PragmaTranslate(CalculatePragma element, TupleList argument, Translator trans)
        {
            foreach (var v in argument)
            {
                Translate((dynamic)v, trans);
            }
            switch (element.Type)
            {
                case CalculatePragmaType.Add: trans.GenerateControl(CodeType.Add); break;
                case CalculatePragmaType.Sub: trans.GenerateControl(CodeType.Sub); break;
                case CalculatePragmaType.Mul: trans.GenerateControl(CodeType.Mul); break;
                case CalculatePragmaType.Div: trans.GenerateControl(CodeType.Div); break;
                case CalculatePragmaType.Mod: trans.GenerateControl(CodeType.Mod); break;
                default: throw new Exception();
            }
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
            trans.GenerateControl(CodeType.Echo);
        }
    }
}
