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
        public TupleList AttributeAccess { get; set; }
        public TupleList DecGeneric { get; set; }
        public TupleList DecArguments { get; set; }
        public Element ExplicitType { get; set; }
        public DirectiveList Block { get; set; }
        public bool IsFunction { get; set; }
        public bool IsDefaultThisReturn { get; private set; }

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
                    _Attribute.Add(v.Reference.FindDataType());
                }
                if(IsFunction)
                {
                    var f = NameResolution("function").FindDataType();
                    _Attribute.Add(f);
                }
                return _Attribute;
            }
        }

        public override IReadOnlyList<IDataType> ArgumentType
        {
            get
            {
                if (_ArgumentType != null)
                {
                    return _ArgumentType;
                }
                _ArgumentType = new List<IDataType>();
                foreach (var v in DecArguments)
                {
                    var temp = v.ReturnType;
                    _ArgumentType.Add(temp);
                }
                return _ArgumentType;
            }
        }

        public override IDataType CallReturnType
        {
            get
            {
                if (_ReturnType != null)
                {
                    return _ReturnType;
                }
                if (ExplicitType != null)
                {
                    _ReturnType = ExplicitType.ReturnType;
                }
                else if (Block.IsInline)
                {
                    var ret = Block[0] as ReturnDirective;
                    if (ret != null)
                    {
                        _ReturnType = ret.Exp.ReturnType;
                    }
                    else
                    {
                        _ReturnType = Block[0].ReturnType;
                    }
                }
                else
                {
                    var ret = Block.FindElements<ReturnDirective>();
                    if (ret.Count > 0)
                    {
                        _ReturnType = ret[0].Exp.ReturnType;
                    }
                    else if(CurrentScope is DeclateClass)
                    {
                        _ReturnType = (DeclateClass)CurrentScope;
                        IsDefaultThisReturn = true;
                    }
                    else
                    {
                        _ReturnType = Root.Void;
                    }
                }
                return _ReturnType;
            }
        }

        public bool IsConstructor
        {
            get
            {
                if(!(CurrentScope is DeclateClass))
                {
                    return false;
                }
                if(Name != "new")
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsDestructor
        {
            get
            {
                if (!(CurrentScope is DeclateClass))
                {
                    return false;
                }
                if (Name != "free")
                {
                    return false;
                }
                return true;
            }
        }

        public override int Count
        {
            get { return 5; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return AttributeAccess;
                    case 1: return DecGeneric;
                    case 2: return DecArguments;
                    case 3: return ExplicitType;
                    case 4: return Block;
                    default: throw new ArgumentOutOfRangeException("index");
                }
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
