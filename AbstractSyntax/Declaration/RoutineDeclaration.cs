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
using AbstractSyntax.Statement;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class RoutineDeclaration : RoutineSymbol
    {
        public TupleLiteral AttributeAccess { get; private set; }
        public TupleLiteral DecGenerics { get; private set; }
        public TupleLiteral DecArguments { get; private set; }
        public Element ExplicitType { get; private set; }

        public RoutineDeclaration(TextPosition tp, string name, RoutineType type, TokenType opType, TupleLiteral attr, TupleLiteral generic, TupleLiteral args, Element expli, ProgramContext block)
            : base(tp, name, type, opType, block)
        {
            AttributeAccess = attr;
            DecGenerics = generic;
            DecArguments = args;
            ExplicitType = expli;
            AppendChild(AttributeAccess);
            AppendChild(DecGenerics);
            AppendChild(DecArguments);
            AppendChild(ExplicitType);
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

        public override IReadOnlyList<ArgumentSymbol> Arguments
        {
            get
            {
                if (_Arguments != null)
                {
                    return _Arguments;
                }
                var a = new List<ArgumentSymbol>();
                foreach (var v in DecArguments)
                {
                    a.Add((ArgumentSymbol)v);
                }
                _Arguments = a;
                return _Arguments;
            }
        }

        public override TypeSymbol CallReturnType
        {
            get
            {
                if (_CallReturnType != null)
                {
                    return _CallReturnType;
                }
                _CallReturnType = Root.Unknown;
                if (ExplicitType != null)
                {
                    _CallReturnType = ExplicitType.OverLoad.FindDataType().Type;
                }
                else if (Block.IsInline)
                {
                    var ret = Block[0] as ReturnStatement;
                    if (ret != null)
                    {
                        _CallReturnType = ret.Exp.ReturnType;
                    }
                    else
                    {
                        _CallReturnType = Block[0].ReturnType;
                    }
                }
                else
                {
                    var ret = Block.FindElements<ReturnStatement>();
                    if (ret.Count > 0)
                    {
                        _CallReturnType = ret[0].Exp.ReturnType;
                    }
                    else if(CurrentScope is ClassDeclaration)
                    {
                        _CallReturnType = (ClassDeclaration)CurrentScope;
                        IsDefaultThisReturn = true;
                    }
                    else
                    {
                        _CallReturnType = Root.Void;
                    }
                }
                return _CallReturnType;
            }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if(CallReturnType is GenericSymbol)
            {
                return;
            }
            if (Block.IsInline)
            {
                var ret = Block[0];
                if (CallReturnType != ret.ReturnType)
                {
                    cmm.CompileError("disagree-return-type", this);
                }
            }
            else
            {
                var ret = Block.FindElements<ReturnStatement>();
                foreach (var v in ret)
                {
                    if (CallReturnType != v.Exp.ReturnType)
                    {
                        cmm.CompileError("disagree-return-type", this);
                    }
                }
            }
        }
    }
}
