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
        public Scope ScopeParent { get; private set; }
        public Root Root { get; private set; }
        public bool IsImport { get; set; }

        public Element()
        {
            Id = NextId++;
        }

        public virtual DataType DataType
        {
            get { throw new NotSupportedException(); }
        }

        public virtual OverLoadScope Reference
        {
            get { throw new NotSupportedException(); }
        }

        public virtual bool IsPragma
        {
            get { return false; }
        }

        public virtual bool IsAssignable
        {
            get { return false; }
        }

        public virtual bool IsVoidValue
        {
            get { return false; }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual Element GetChild(int index)
        {
            throw new ArgumentOutOfRangeException();
        }

        public Element this[int index]
        {
            get { return GetChild(index); }
        }

        public IEnumerator<Element> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetChild(i);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual string AdditionalInfo()
        {
            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Position).Append(" ").Append(this.GetType().Name);
            var add = AdditionalInfo();
            if(add != null)
            {
                builder.Append(": ").Append(add);
            }
            return builder.ToString();
        }

        private void PostCompileInfo(string key, CompileMessageType type)
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

        protected void CompileInfo(string key)
        {
            PostCompileInfo(key, CompileMessageType.Info);
        }

        protected void CompileError(string key)
        {
            PostCompileInfo(key, CompileMessageType.Error);
        }

        protected void CompileWarning(string key)
        {
            PostCompileInfo(key, CompileMessageType.Warning);
        }

        protected void SpreadElement(Element parent, Scope scope)
        {
            Parent = parent;
            ScopeParent = scope;
            if (this is Root)
            {
                Root = (Root)this;
            }
            else
            {
                Root = parent.Root;
            }
            if(this is Scope)
            {
                var temp = (Scope)this;
                foreach (Element v in this)
                {
                    if (v != null)
                    {
                        v.SpreadElement(this, temp);
                    }
                }
                temp.SpreadScope(scope);
            }
            else
            {
                foreach (Element v in this)
                {
                    if (v != null)
                    {
                        v.SpreadElement(this, scope);
                    }
                }
            }
        }

        internal virtual void SpreadReference(Scope scope)
        {
            if (this is Scope)
            {
                scope = (Scope)this;
            }
            foreach (Element v in this)
            {
                if (v != null)
                {
                    v.SpreadReference(scope);
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
    }
}
