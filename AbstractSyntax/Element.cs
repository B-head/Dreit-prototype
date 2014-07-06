using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AbstractSyntax
{
    [DebuggerVisualizer(typeof(SyntaxVisualizer))]
    [Serializable]
    public abstract class Element : IReadOnlyList<Element>
    {
        public TextPosition Position { get; private set; }
        internal Element Parent { get; private set; }
        internal Scope CurrentScope { get; private set; }
        internal Root Root { get; private set; }

        protected Element()
        {

        }

        protected Element(TextPosition tp)
        {
            Position = tp;
        }

        public Scope CurrentIScope 
        {
            get { return CurrentScope; }
        }

        public virtual Scope ReturnType
        {
            get { return Root.Void; }
        }

        public virtual OverLoad Reference
        {
            get { return Root.UndefinedOverLord; }
        }

        public bool IsVoidReturn
        {
            get { return ReturnType is VoidSymbol; }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual Element this[int index]
        {
            get { throw new ArgumentOutOfRangeException("index"); }
        }

        internal virtual void SpreadElement(Element parent, Scope scope)
        {
            Parent = parent;
            CurrentScope = scope;
            var root = this as Root;
            if (root != null)
            {
                Root = root;
            }
            else
            {
                if(parent == null)
                {
                    throw new ArgumentException("parent");
                }
                Root = parent.Root;
            }
            var s = this as Scope;
            if (s != null)
            {
                scope = s;
            }
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.SpreadElement(this, scope);
                }
            }
        }

        internal virtual void CheckSemantic()
        {
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.CheckSemantic();
                }
            }
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

        protected void CompileInfo(string key)
        {
            SendCompileInfo(key, CompileMessageType.Info);
        }

        protected void CompileError(string key)
        {
            SendCompileInfo(key, CompileMessageType.Error);
        }

        protected void CompileWarning(string key)
        {
            SendCompileInfo(key, CompileMessageType.Warning);
        }

        private void SendCompileInfo(string key, CompileMessageType type)
        {
            CompileMessage info = new CompileMessage
            {
                MessageType = type,
                Key = key,
                Position = Position,
                Target = this,
            };
            Root.MessageManager.Append(info);
        }

        protected virtual string ElementInfo
        {
            get { return null; }
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
}
