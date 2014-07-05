using AbstractSyntax.Daclate;
using AbstractSyntax.Statement;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class DirectiveList : Element
    {
        private List<Element> Child;
        public bool IsInline { get; private set; }

        public DirectiveList()
        {
            Child = new List<Element>();
        }

        public DirectiveList(TextPosition tp, List<Element> child, bool isInline)
            :base(tp)
        {
            Child = child;
            IsInline = isInline;
        }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override Element this[int index]
        {
            get { return Child[index]; }
        }

        protected override string ElementInfo
        {
            get { return "Count = " + Child.Count; }
        }

        public bool IsNoReturn
        {
            get 
            { 
                if(Parent is IfStatement || Parent is LoopStatement)
                {
                    return true;
                }
                return Parent is NameSpace && !(Parent is DeclateModule); 
            }
        }

        public void Append(Element value)
        {
            Child.Add(value);
        }

        public IReadOnlyList<T> FindElements<T>() where T : Element
        {
            var result = new List<T>();
            foreach(var v in Child)
            {
                if(v is T)
                {
                    result.Add((T)v);
                }
            }
            return result;
        }
    }
}
