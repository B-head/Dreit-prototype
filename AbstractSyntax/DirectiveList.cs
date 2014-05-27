using AbstractSyntax.Daclate;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class DirectiveList : Element
    {
        private List<IElement> Child;
        public bool IsInline { get; set; }

        public DirectiveList()
        {
            Child = new List<IElement>();
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override IElement this[int index]
        {
            get { return Child[index]; }
        }

        public bool IsNoReturn
        {
            get { return Parent is NameSpace && !(Parent is DeclateModule); }
        }

        public bool IsThisReturn
        {
            get
            {
                var rout = Parent as DeclateRoutine;
                return rout != null && rout.IsThisReturn;
            }
        }

        public void Append(IElement append)
        {
            Child.Add(append);
        }

        public IReadOnlyList<E> FindElements<E>() where E : Element
        {
            var result = new List<E>();
            foreach(var v in Child)
            {
                if(v is E)
                {
                    result.Add((E)v);
                }
            }
            return result;
        }
    }
}
