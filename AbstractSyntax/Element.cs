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
        private static int NextId = 1;
        public int Id { get; private set; }
        public TextPosition Position { get; set; }
        public Element Parent { get; private set; }
        public Scope CurrentScope { get; private set; }
        public Root Root { get; private set; }
        public bool IsImport { get; set; }

        public Element()
        {
            Id = NextId++;
        }

        public virtual DataType DataType
        {
            get { return Root.Void; }
        }

        public virtual bool IsVoidValue
        {
            get { return DataType is VoidSymbol; }
        }

        protected virtual string ElementInfo
        {
            get { return null; }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual Element this[int index]
        {
            get { throw new ArgumentOutOfRangeException(); }
        }

        protected virtual void SpreadElement(Element parent, Scope scope)
        {
            Parent = parent;
            CurrentScope = scope;
            if (this is Root)
            {
                Root = (Root)this;
            }
            else
            {
                Root = parent.Root;
            }
            if (this is Scope)
            {
                scope = (Scope)this;
            }
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.SpreadElement(this, scope);
                }
            }
        }

        internal virtual void CheckSyntax()
        {
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.CheckSyntax();
                }
            }
        }

        internal virtual void CheckDataType()
        {
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.CheckDataType();
                }
            }
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
                Type = type,
                Key = key,
                Position = Position,
                Target = this,
            };
            Root.MessageManager.Append(info);
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
