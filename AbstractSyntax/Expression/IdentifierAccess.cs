using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class IdentifierAccess : Element, IAccess
    {
        public string Value { get; set; }
        public bool IsPragmaAccess { get; set; }
        private bool? _IsTacitThis;
        private OverLoad _Reference;

        public bool IsTacitThis
        {
            get
            {
                if (_IsTacitThis != null)
                {
                    return _IsTacitThis.Value;
                }
                var refer = CallScope;
                if (refer == null || refer.CurrentScope != GetParentClass() || refer is ThisSymbol)
                {
                    _IsTacitThis = false;
                }
                else if (!(CurrentScope is DeclateRoutine) || Parent is MemberAccess)
                {
                    _IsTacitThis = false;
                }
                else
                {
                    _IsTacitThis = true;
                }
                return _IsTacitThis.Value;
            }
        }

        public ThisSymbol ThisReference
        {
            get
            {
                var c = GetParentClass() as DeclateClass;
                return c.This;
            }
        }

        protected override string GetElementInfo()
        {
            if (IsPragmaAccess)
            {
                return "@@" + Value;
            }
            else
            {
                return Value;
            }
        }

        public override IDataType DataType
        {
            get { return Reference.GetDataType(); }
        }

        public Scope CallScope
        {
            get { return Reference.TypeSelect().Call; }
        }

        public OverLoad Reference
        {
            get
            {
                if(_Reference == null)
                {
                    RefarenceResolution(CurrentScope);
                }
                return _Reference;
            }
        }

        public void RefarenceResolution(IScope scope)
        {
            if (IsPragmaAccess)
            {
                _Reference = ((Scope)scope).NameResolution("@@" +  Value);
            }
            else
            {
                _Reference = ((Scope)scope).NameResolution(Value);
            }
        }

        private Scope GetParentClass()
        {
            var current = CurrentScope;
            while(current != null)
            {
                if(current is DeclateClass)
                {
                    break;
                }
                current = current.CurrentScope;
            }
            return current;
        }

        internal override void CheckDataType()
        {
            if (Reference is UnknownOverLoad)
            {
                if (IsPragmaAccess)
                {
                    CompileError("undefined-pragma");
                }
                else
                {
                    CompileError("undefined-identifier");
                }
            }
            base.CheckDataType();
        }
    }
}
