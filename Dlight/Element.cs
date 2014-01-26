using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight
{
    abstract class Element : IEnumerable<Element>
    {
        public virtual int Count
        {
            get { return 0; }
        }

        public virtual Element this[int index]
        {
            get { throw new ArgumentOutOfRangeException(); }
        }

        public virtual string ElementInfo()
        {
            return this.GetType().Name + ": ";
        }

        public virtual string ErrorInfo()
        {
            return string.Empty;
        }

        public virtual void CreateScope(Scope<Element> scope)
        {
            foreach(Element v in this)
            {
                if(v != null)
                {
                    v.CreateScope(scope);
                }
            }
        }

        public virtual void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            foreach (Element v in this)
            {
                if (v == null)
                {
                    manager.Error(ErrorInfo() + "原因不明の<null>要素が検出されました。");
                    continue;
                }
                v.CheckSemantic(manager, scope);
            }
        }

        public virtual void Translate(Translator trans, Scope<Element> scope)
        {
            foreach(Element v in this)
            {
                v.Translate(trans, scope);
            }
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public virtual string ToString(int indent)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(Common.Indent(indent) + ElementInfo());
            foreach (Element v in this)
            {
                if (v == null)
                {
                    result.AppendLine("<null>");
                    continue;
                }
                result.Append(v.ToString(indent + 1));
            }
            return result.ToString();
        }

        public IEnumerator<Element> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    abstract class Syntax : Element
    {
        public TextPosition Position { get; set; }

        public override string ElementInfo()
        {
            return Position + " " + base.ElementInfo();
        }

        public override string ErrorInfo()
        {
            return base.ErrorInfo() + Position + ": ";
        }
    }

    abstract class Reference : Syntax
    {
        public override void Translate(Translator trans, Scope<Element> scope)
        {
            Translate(trans, scope, false);
        }

        public virtual void Translate(Translator trans, Scope<Element> scope, bool assign)
        {
            foreach (Element v in this)
            {
                Reference temp = v as Reference;
                if (temp == null)
                {
                    v.Translate(trans, scope);
                }
                else
                {
                    temp.Translate(trans, scope, assign);
                }
            }
        }
    }

    class AssemblyElement : Element
    {
        public string Name { get; set; }
        public List<ModuleElement> Child { get; set; }
        public Scope<Element> Scope { get; set; }

        public AssemblyElement(string name, List<ModuleElement> child)
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

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope = null)
        {
            base.CheckSemantic(manager, Scope);
        }

        public override void Translate(Translator trans, Scope<Element> scope = null)
        {
            base.Translate(trans, Scope);
        }
    }

    class ModuleElement : Element
    {
        public string Name { get; set; }
        public List<Syntax> Child { get; set; }
        public List<Token> ErrorToken { get; set; }
        public Scope<Element> Scope { get; set; }

        public ModuleElement(string name, List<Syntax> child, List<Token> error)
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

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            foreach(Token v in ErrorToken)
            {
                if (v.Type == TokenType.OtherString)
                {
                    manager.Error(base.ErrorInfo() + v.Position + ": 文字列 " + v.Text + " は有効なトークンではありません。");
                }
                else
                {
                    manager.Error(base.ErrorInfo() + v.Position + ": トークン " + v.Text + " をこの位置に書くことは出来ません。");
                }
            }
            base.CheckSemantic(manager, Scope);
        }

        public override void Translate(Translator trans, Scope<Element> scope)
        {
            Translator temp = trans.CreateModule(Scope);
            base.Translate(temp, Scope);
        }
    }
}
