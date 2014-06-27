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
        public TupleList AttributeAccess { get; set; }
        public TupleList DecGeneric { get; set; }
        public TupleList InheritAccess { get; set; }
        public DirectiveList Block { get; set; }
        public ThisSymbol This { get; private set; }
        public DefaultSymbol Default { get; private set; }

        public DeclateClass()
        {
            This = new ThisSymbol(this);
            Default = new DefaultSymbol(this);
        }

        public override IReadOnlyList<IScope> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                _Attribute = new List<IScope>();
                foreach (var v in AttributeAccess)
                {
                    var acs = v as IAccess;
                    if (acs != null)
                    {
                        _Attribute.Add(acs.Reference.SelectPlain());
                    }
                }
                return _Attribute;
            }
        }

        public override IReadOnlyList<ClassSymbol> Inherit 
        {
            get
            {
                if (_Inherit != null)
                {
                    return _Inherit;
                }
                _Inherit = new List<ClassSymbol>();
                foreach (var v in InheritAccess)
                {
                    if (v.DataType is ClassSymbol)
                    {
                        _Inherit.Add((ClassSymbol)v.DataType);
                    }
                }
                return _Inherit;
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
            get { return 6; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return AttributeAccess;
                    case 1: return DecGeneric;
                    case 2: return InheritAccess;
                    case 3: return Block;
                    case 4: return This;
                    case 5: return Default;
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
            foreach (var v in InheritAccess)
            {
                if (!(v.DataType is ClassSymbol))
                {
                    CompileError("not-datatype-inherit");
                }
            }
        }
    }
}
