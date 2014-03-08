using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;
using Common;

namespace CliTranslate
{
    public class TranslateManager
    {
        private Dictionary<Scope, Translator> TransDictionary;
        private RootTranslator Root;

        public TranslateManager(string name)
        {
            TransDictionary = new Dictionary<Scope, Translator>();
            Root = new RootTranslator(name);
        }

        public void TranslateTo(Root root, ImportManager manager)
        {
            manager.TranslateImport(Root);
            SpreadTranslate(root, Root);
            Translate(root, Root);
        }

        internal virtual void ChildSpreadTranslate(Scope scope, Translator trans)
        {
            foreach (var v in scope.ScopeChild.Values)
            {
                if (v == null || v.IsImport)
                {
                    continue;
                }
                Translate((dynamic)v, trans);
            }
        }

        internal virtual void SpreadTranslate(Scope scope, Translator trans)
        {
            ChildSpreadTranslate(scope, trans);
        }

        internal virtual void SpreadTranslate(DeclateModule scope, Translator trans)
        {
            var temp = trans.CreateModule(scope.FullPath);
            TransDictionary.Add(scope, temp);
            ChildSpreadTranslate(scope, temp);
        }

        internal virtual void SpreadTranslate(DeclateClass scope, Translator trans)
        {
            var temp = trans.CreateClass(scope.FullPath);
            TransDictionary.Add(scope, temp);
            ChildSpreadTranslate(scope, temp);
        }

        internal virtual void SpreadTranslate(DeclateRoutine scope, Translator trans)
        {
            var temp = trans.CreateRoutine(scope.FullPath, scope.ReturnType.FullPath);
            TransDictionary.Add(scope, temp);
            ChildSpreadTranslate(scope, temp);
            temp.SaveArgument();
        }

        internal virtual void SpreadTranslate(DeclateVariant scope, Translator trans)
        {
            trans.CreateVariant(scope.FullPath, scope.DataType.FullPath);
            ChildSpreadTranslate(scope, trans);
        }

        internal virtual void SpreadTranslate(DeclateArgument scope, Translator trans)
        {
            RoutineTranslator routTrans = trans as RoutineTranslator;
            routTrans.CreateArgument(scope.FullPath, scope.DataType.FullPath);
            ChildSpreadTranslate(scope, trans);
        }

        private void ChildTranslate(Element element, Translator trans)
        {
            foreach(var v in element)
            {
                if (v == null || v.IsImport)
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
                if (!v.IsVoidValue && element.IsInline)
                {
                    trans.GenerateControl(CodeType.Pop);
                }
            }
            if (element.Count <= 0 || !(element.GetChild(element.Count - 1) is ReturnDirective))
            {
                trans.GenerateControl(CodeType.Ret);
            }
        }

        private void Translate(DeclateClass element, Translator trans)
        {
            var temp = TransDictionary[element];
            Translate(element.Block, temp);
        }

        private void Translate(DeclateRoutine element, Translator trans)
        {
            var temp = TransDictionary[element];
            Translate(element.Block, temp);
        }

        private void Translate(DeclateVariant element, Translator trans)
        {
            Translate(element.Ident, trans);
        }

        private void Translate(IdentifierAccess element, Translator trans)
        {
            trans.GenerateLoad(element.Refer.FullPath);
        }

        private void TranslateAssign(IdentifierAccess element, Translator trans)
        {
            trans.GenerateStore(element.Refer.FullPath);
        }

        private Element TranslateAccess(MemberAccess element, Translator trans)
        {
            Translate(element.Left, trans);
            var temp = element.Right as MemberAccess;
            if (temp != null)
            {
                return TranslateAccess(temp, trans);
            }
            return element.Right;
        }

        private void Translate(DyadicCalculate element, Translator trans)
        {
            ChildTranslate(element, trans);
            trans.GenerateCall(element.ReferOp.FullPath);
        }

        private void Translate(LeftAssign element, Translator trans)
        {
            var member = element.Left as MemberAccess;
            var refer = member == null ? element.Left : TranslateAccess(member, trans);
            Translate(element.Right, trans);
            TranslateAssign((IdentifierAccess)refer, trans);
            Translate((IdentifierAccess)refer, trans);
        }

        private void Translate(RightAssign element, Translator trans)
        {
            var member = element.Right as MemberAccess;
            var refer = member == null ? element.Right : TranslateAccess(member, trans);
            Translate(element.Left, trans);
            TranslateAssign((IdentifierAccess)refer, trans);
            Translate((IdentifierAccess)refer, trans);
        }

        private void Translate(NumberLiteral element, Translator trans)
        {
            dynamic number = element.Parse();
            trans.GeneratePrimitive(number);
            trans.GenerateCall(element.DataType.FullPath);
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
