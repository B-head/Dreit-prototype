using AbstractSyntax.Directive;
using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateRoutine : RoutineSymbol
    {
        public TupleList AttributeAccess { get; private set; }
        public TupleList DecGeneric { get; private set; }
        public TupleList DecArguments { get; private set; }
        public Element ExplicitType { get; private set; }
        public DirectiveList Block { get; private set; }
        public bool IsDefaultThisReturn { get; private set; }

        public DeclateRoutine(TextPosition tp, string name, TokenType op, bool isFunc, TupleList attr, TupleList generic, TupleList args, Element expl, DirectiveList block)
            : base(tp, name, op, isFunc)
        {
            AttributeAccess = attr;
            DecGeneric = generic;
            DecArguments = args;
            ExplicitType = expl;
            Block = block;
            AppendChild(AttributeAccess);
            AppendChild(DecGeneric);
            AppendChild(DecArguments);
            AppendChild(ExplicitType);
            AppendChild(Block);
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
                if(IsFunction)
                {
                    a.Add(Root.Function);
                }
                else
                {
                    a.Add(Root.Routine);
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

        public override IReadOnlyList<Scope> ArgumentTypes
        {
            get
            {
                if (_ArgumentTypes != null)
                {
                    return _ArgumentTypes;
                }
                var a = new List<Scope>();
                foreach (var v in DecArguments)
                {
                    var temp = v.ReturnType;
                    a.Add(temp);
                }
                _ArgumentTypes = a;
                return _ArgumentTypes;
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
                    _CallReturnType = ExplicitType.ReturnType;
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
                    else if(CurrentScope is DeclateClass)
                    {
                        _CallReturnType = (DeclateClass)CurrentScope;
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

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
            if (Block.IsInline)
            {
                var ret = Block[0];
                if (CallReturnType != ret.ReturnType)
                {
                    CompileError("disagree-return-type");
                }
            }
            else
            {
                var ret = Block.FindElements<ReturnDirective>();
                foreach (var v in ret)
                {
                    if (CallReturnType != v.Exp.ReturnType)
                    {
                        CompileError("disagree-return-type");
                    }
                }
            }
        }
    }
}
