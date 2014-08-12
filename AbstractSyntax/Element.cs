using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AbstractSyntax
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer), typeof(SyntaxVisualizerSource))]
    [Serializable]
    public abstract class Element : IReadOnlyTree<Element>
    {
        private Root _Root;
        private Scope _CurrentScope;
        private List<Element> Child;
        public Element Parent { get; private set; }
        public TextPosition Position { get; private set; }

        protected Element()
        {
            Child = new List<Element>();
            var root = this as Root;
            if (root != null)
            {
                _Root = root;
            }
        }

        protected Element(TextPosition tp)
        {
            Child = new List<Element>();
            Position = tp;
        }
        
        public virtual Scope ReturnType
        {
            get { return Root.Void; }
        }

        public virtual OverLoadReference OverLoad
        {
            get { return Root.UndefinedOverLord; }
        }

        public virtual bool IsConstant
        {
            get { return false; }
        }

        public bool IsVoidReturn
        {
            get { return ReturnType is VoidSymbol; }
        }

        public Root Root
        {
            get
            {
                if (_Root == null)
                {
                    _Root = Parent.Root;
                }
                return _Root;
            }
        }

        public Scope CurrentScope
        {
            get
            {
                if(_CurrentScope == null)
                {
                    var c = Parent as Scope;
                    if (c != null)
                    {
                        _CurrentScope = c;
                    }
                    else if (Parent != null)
                    {
                        _CurrentScope = Parent.CurrentScope;
                    }
                }
                return _CurrentScope;
            }
        }

        internal void AppendChild(IEnumerable<Element> childs)
        {
            if(childs == null)
            {
                return;
            }
            foreach(var v in childs)
            {
                AppendChild(v);
            }
        }

        internal void AppendChild(Element child)
        {
            if (child == null)
            {
                return;
            }
            Child.Add(child);
            child.RegisterParent(this);
            var s = this as Scope;
            if(s == null)
            {
                return;
            }
            s.SpreadChildScope(child);
        }

        private void RegisterParent(Element parent)
        {
            if(Parent != null)
            {
                throw new InvalidOperationException();
            }
            Parent = parent;
            var s = this as Scope;
            if(s == null)
            {
                return;
            }
            var cs = CurrentScope;
            if(cs == null)
            {
                return;
            }
            cs.AppendChildScope(s);
        }

        internal virtual void CheckSemantic(CompileMessageManager cmm)
        {
            return;
        }

        internal bool HasCurrentAccess(Scope other)
        {
            var c = CurrentScope;
            while (c != null)
            {
                if (c == other)
                {
                    return true;
                }
                c = c.CurrentScope;
            }
            return false;
        }

        internal static bool HasAnyAttribute(IReadOnlyList<Scope> attribute, params AttributeType[] type)
        {
            foreach (var v in attribute)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                if (type.Any(t => t == a.AttributeType))
                {
                    return true;
                }
            }
            return false;
        }

        internal T GetParent<T>() where T : Scope
        {
            var current = CurrentScope;
            while (current != null)
            {
                if (current is T)
                {
                    break;
                }
                current = current.CurrentScope;
            }
            return current as T;
        }

        protected virtual string ElementInfo
        {
            get { return "Count = " + Count; }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Position).Append(" ").Append(this.GetType().Name);
            var add = ElementInfo;
            if (add != null)
            {
                builder.Append(": ").Append(add);
            }
            return builder.ToString();
        }

        public int Count
        {
            get { return Child.Count; }
        }

        public Element this[int index]
        {
            get { return Child[index]; }
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return Child.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        Element IReadOnlyTree<Element>.Root
        {
            get { return Root; }
        }
    }
}
