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
        private RootTranslator Root;

        public TranslateManager(string name)
        {
            Root = new RootTranslator(name);
        }

        public void TranslateTo(Root root, ImportManager manager)
        {
            manager.TranslateImport(Root);
            Translate(root, Root);
        }

        private void TranslateChild(Element ele, Translator trans)
        {
            foreach(var v in ele)
            {
                Translate((dynamic)ele, trans);
            }
        }

        private void Translate(Element ele, Translator trans)
        {
            TranslateChild(ele, trans);
        }
    }
}
