using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class NameSpace : Scope
    {
        public List<Element> Child { get; set; }

        public NameSpace()
        {
            Child = new List<Element>();
        }

        public void Append(Element append)
        {
            Child.Add(append);
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return Child[index];
        }

        internal override bool IsNameSpace
        {
            get { return true; }
        }

        internal override void Translate(Translator trans)
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    continue;
                }
                if (v is NameSpace || v is DeclateModule)
                {
                    v.Translate(trans);
                }
            }
        }
    }
}
