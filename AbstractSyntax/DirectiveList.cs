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
        public bool IsInline { get; private set; }

        public DirectiveList()
        {

        }

        public DirectiveList(TextPosition tp, List<Element> child, bool isInline)
            :base(tp)
        {
            IsInline = isInline;
            AppendChild(child);
        }

        protected override string ElementInfo
        {
            get { return "Count = " + Count; }
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

        public IReadOnlyList<T> FindElements<T>() where T : Element
        {
            var result = new List<T>();
            foreach(var v in this)
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
