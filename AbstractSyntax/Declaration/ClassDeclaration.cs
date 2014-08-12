using AbstractSyntax.Expression;
using AbstractSyntax.Literal;
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
        public TupleLiteral AttributeAccess { get; private set; }
        public TupleLiteral DecGenerics { get; private set; }
        public TupleLiteral InheritAccess { get; private set; }

        public ClassDeclaration(TextPosition tp, string name, ClassType type, TupleLiteral attr, TupleLiteral generic, TupleLiteral inherit, ProgramContext block)
            :base(tp, name, type, block)
        {
            AttributeAccess = attr;
            DecGenerics = generic;
            InheritAccess = inherit;
            AppendChild(AttributeAccess);
            AppendChild(DecGenerics);
            AppendChild(InheritAccess);
        }

        public override bool IsConstant
        {
            get { return true; }
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
            foreach(var v in Block)
            {
                if(!v.IsConstant)
                {
                    cmm.CompileError("not-constant-expression", v);
                }
            }
        }
    }
}
