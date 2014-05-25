using AbstractSyntax.Expression;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateClass : ClassSymbol
    {
        public TupleList Generic { get; set; }
        public TupleList Inherit { get; set; }
        public DirectiveList Block { get; set; }
        public ThisSymbol This { get; private set; }
        public DefaultSymbol Default { get; private set; }

        public DeclateClass()
        {
            This = new ThisSymbol(this);
            Default = new DefaultSymbol(this);
        }

        public override List<ClassSymbol> InheritRefer 
        {
            get
            {
                if (_InheritRefer != null)
                {
                    return _InheritRefer;
                }
                _InheritRefer = new List<ClassSymbol>();
                foreach (var v in Inherit)
                {
                    if (v.DataType is ClassSymbol)
                    {
                        InheritRefer.Add((ClassSymbol)v.DataType);
                    }
                }
                return _InheritRefer;
            }
        }

        public override bool IsVoidValue
        {
            get { return true; } //todo この代わりのプロパティが必要。
        }

        public bool IsDefaultConstructor
        {
            get { return initializer.Any(v => v is DefaultSymbol); }
        }

        public override int Count
        {
            get { return 5; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Generic;
                    case 1: return Inherit;
                    case 2: return Block;
                    case 3: return This;
                    case 4: return Default;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void SpreadElement(Element parent, Scope scope)
        {
            base.SpreadElement(parent, scope);
            foreach(var e in Block)
            {
                var r = e as RoutineSymbol;
                if (r == null)
                {
                    continue;
                }
                if(r.Name == "new")
                {
                    initializer.Add(r);
                }
                else if (r.Name == "from")
                {
                    Root.Conversion.Append(r);
                    initializer.Add(r);
                }
                else if (r.Operator != TokenType.Unknoun)
                {
                    Root.OpManager[r.Operator].Append(r);
                }
            }
            if(initializer.Count == 0)
            {
                initializer.Add(Default);
            }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            foreach (var v in Inherit)
            {
                if (!(v.DataType is ClassSymbol))
                {
                    CompileError("not-datatype-inherit");
                }
            }
        }
    }
}
