using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class DirectiveList : Element
    {
        public List<Element> _Child { get; set; }

        public DirectiveList()
        {
            _Child = new List<Element>();
        }

        public void Append(Element append)
        {
            if (append == null)
            {
                return;
            }
            if(_Child.Count == 0)
            {
                Position = append.Position;
            }
            _Child.Add(append);
        }

        public override int Count
        {
            get { return _Child.Count; }
        }

        public override Element Child(int index)
        {
            return _Child[index];
        }

        internal override void Translate(Translator trans)
        {
            foreach(Element v in this)
            {
                v.Translate(trans);
                trans.GenelateControl(CodeType.Pop);
            }
            trans.GenelateControl(CodeType.Ret);
        }
    }
}
