using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight
{
    class Assembly : Element
    {
        public string Name { get; set; }
        public List<Module> Child { get; set; }
        public Scope<Element> Scope { get; set; }

        public Assembly(string name, List<Module> child)
        {
            Name = name;
            Child = child;
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element this[int index]
        {
            get { return Child[index]; }
        }

        public override string ElementInfo()
        {
            return Name + " " + base.ElementInfo();
        }

        public override string ErrorInfo()
        {
            return base.ErrorInfo() + ElementInfo();
        }

        public override void CreateScope(Scope<Element> scope = null)
        {
            Scope = new Scope<Element>(this); //名前の登録はしない。
            base.CreateScope(Scope);
        }

        public override void Translate(Translator trans)
        {
            base.Translate(trans);
            trans.Save();
        }
    }

    class Module : Element
    {
        public string Name { get; set; }
        public List<Syntax> Child { get; set; }
        public List<Token> ErrorToken { get; set; }
        public Scope<Element> Scope { get; set; }

        public Module(string name, List<Syntax> child, List<Token> error)
        {
            Name = name;
            Child = child.ToList();
            ErrorToken = error.ToList();
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element this[int index]
        {
            get { return Child[index]; }
        }

        public override string ElementInfo()
        {
            return Name + " " + base.ElementInfo() + "ErrorToken = " + ErrorToken.Count.ToString();
        }

        public override string ErrorInfo()
        {
            return base.ErrorInfo() + ElementInfo();
        }

        public override void CreateScope(Scope<Element> scope)
        {
            Scope = scope.CreateChild(this, Name);
            base.CreateScope(Scope);
        }

        public override void Translate(Translator trans)
        {
            Translator temp = trans.CreateModule(Scope);
            base.Translate(temp);
        }
    }
}
