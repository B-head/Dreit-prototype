/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get
            {
                if (_Attribute != null)
                {
                    return _Attribute;
                }
                var a = new List<AttributeSymbol>();
                foreach (var v in AttributeAccess)
                {
                    a.Add(v.OverLoad.FindAttribute());
                }
                if (!a.HasAnyAttribute(AttributeType.Public, AttributeType.Protected, AttributeType.Private))
                {
                    var p = NameResolution("public").FindAttribute();
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

        public override IReadOnlyList<TypeSymbol> Inherit 
        {
            get
            {
                if (_Inherit != null)
                {
                    return _Inherit;
                }
                var i = new List<TypeSymbol>();
                foreach (var v in InheritAccess)
                {
                    var dt = v.OverLoad.FindDataType();
                    i.Add(dt.Type);
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
                if (!(dt.Type is ClassSymbol))
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
