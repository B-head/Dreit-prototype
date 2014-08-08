using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class ClassDeclaration : ClassSymbol
    {
        public TupleList AttributeAccess { get; private set; }
        public TupleList DecGenerics { get; private set; }
        public TupleList InheritAccess { get; private set; }

        public ClassDeclaration(TextPosition tp, string name, bool isTrait, TupleList attr, TupleList generic, TupleList inherit, DirectiveList block)
            :base(tp, name, isTrait, block)
        {
            AttributeAccess = attr;
            DecGenerics = generic;
            InheritAccess = inherit;
            AppendChild(AttributeAccess);
            AppendChild(DecGenerics);
            AppendChild(InheritAccess);
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                var a = new List<Scope>();
                foreach (var v in AttributeAccess)
                {
                    a.Add(v.OverLoad.FindDataType());
                }
                if (!HasAnyAttribute(a, AttributeType.Public, AttributeType.Protected, AttributeType.Private))
                {
                    var p = NameResolution("public").FindDataType();
                    a.Add(p);
                }
                _Attribute = a;
                return _Attribute;
            }
        }

        public override IReadOnlyList<GenericSymbol> Generics
        {
            get
            {
                if (_Generics != null)
                {
                    return _Generics;
                }
                var pt = new List<GenericSymbol>();
                foreach (var v in DecGenerics)
                {
                    pt.Add((GenericSymbol)v);
                }
                _Generics = pt;
                return _Generics;
            }
        }

        public override IReadOnlyList<Scope> Inherit 
        {
            get
            {
                if (_Inherit != null)
                {
                    return _Inherit;
                }
                var i = new List<Scope>();
                foreach (var v in InheritAccess)
                {
                    var dt = v.OverLoad.FindDataType();
                    i.Add(dt);
                }
                _Inherit = i;
                return _Inherit;
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            foreach (var v in InheritAccess)
            {
                var dt = v.OverLoad.FindDataType();
                if (!(dt is ClassSymbol))
                {
                    cmm.CompileError("not-datatype-inherit", this);
                }
            }
        }
    }
}
