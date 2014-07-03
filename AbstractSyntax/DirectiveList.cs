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

        public bool IsThisReturn
        {
            get
            {
                var rout = Parent as DeclateRoutine;
                return rout != null && rout.IsThisReturn;
            }
        }

        public void Append(IElement value)
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
