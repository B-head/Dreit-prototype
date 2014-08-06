using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
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
        public TupleList AttributeAccess { get; private set; }
        public TupleList DecGenerics { get; private set; }
        public TupleList DecArguments { get; private set; }
        public Element ExplicitType { get; private set; }
        public bool IsDefaultThisReturn { get; private set; }

        public RoutineDeclaration(TextPosition tp, string name, TokenType op, bool isFunc, TupleList attr, TupleList generic, TupleList args, Element expl, DirectiveList block)
            : base(tp, name, op, isFunc, block)
        {
            AttributeAccess = attr;
            DecGenerics = generic;
            DecArguments = args;
            ExplicitType = expl;
            AppendChild(AttributeAccess);
            AppendChild(DecGenerics);
            AppendChild(DecArguments);
            AppendChild(ExplicitType);
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
                if(!HasAnyAttribute(a, AttributeType.Public, AttributeType.Protected, AttributeType.Private))
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

        public override Scope CallReturnType
        {
            get
            {
                if (_CallReturnType != null)
                {
                    return _CallReturnType;
                }
                if (ExplicitType != null)
                {
                    _CallReturnType = ExplicitType.OverLoad.FindDataType();
                }
                else if (Block.IsInline)
                {
                    var ret = Block[0] as ReturnDirective;
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
                    var ret = Block.FindElements<ReturnDirective>();
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

        public override bool IsVirtual //todo オーバーライドされる可能性が無ければnon-virtualにする。
        {
            get
            {
                var cls = GetParent<ClassSymbol>();
                if (cls == null)
                {
                    return false;
                }
                return IsInstanceMember && !cls.IsPrimitive;
            }
        }

        public override bool IsAbstract
        {
            get { return Block == null; }
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
                var ret = Block.FindElements<ReturnDirective>();
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
