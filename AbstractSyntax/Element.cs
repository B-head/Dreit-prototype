using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AbstractSyntax
{
    public interface IElement : IReadOnlyList<IElement>
    {
        TextPosition Position { get; }
        IScope CurrentIScope { get; }
        IDataType DataType { get; }
        bool IsVoidReturn { get; }
    }

    [DebuggerVisualizer(typeof(SyntaxVisualizer))]
    [Serializable]
    public abstract class Element : IElement
    {
        public TextPosition Position { get; set; }
        internal Element Parent { get; private set; }
        internal Scope CurrentScope { get; private set; }
        internal Root Root { get; private set; }

        public IScope CurrentIScope 
        {
            get { return CurrentScope; }
        }

        public virtual IDataType DataType
        {
            get { return Root.Void; }
        }

        public bool IsVoidReturn
        {
            get { return DataType is VoidSymbol; }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual IElement this[int index]
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

        protected virtual string GetElementInfo()
        {
            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Position).Append(" ").Append(this.GetType().Name);
            var add = GetElementInfo();
            if (add != null)
            {
                builder.Append(": ").Append(add);
            }
            return builder.ToString();
        }

        public IEnumerator<IElement> GetEnumerator()
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
