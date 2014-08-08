using AbstractSyntax.Declaration;
using AbstractSyntax.Statement;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Directive
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

        public void Append(Element child)
        {
            AppendChild(child);
        }

        public override bool IsConstant
        {
            get
            {
                foreach (var v in this)
                {
                    if (!v.IsConstant)
                    {
                        return false;
                    }
                }
                return true;
            }
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
                return Parent is NameSpaceSymbol && !(Parent is ModuleDeclaration); 
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
