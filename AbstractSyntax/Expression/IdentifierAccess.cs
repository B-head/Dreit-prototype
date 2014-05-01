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
                var voidSelect = ScopeReference;
                if (voidSelect != null && voidSelect.CurrentScope == GetParentClass() && CurrentScope is DeclateRoutine)
                {
                    _IsTacitThis = true;
                }
                else
                {
                    _IsTacitThis = false;
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

        protected override string ElementInfo
        {
            get
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
        }

        public override DataType DataType
        {
            get { return Reference.GetDataType(); }
        }

        public Scope ScopeReference
        {
            get { return Reference.TypeSelect(); }
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

        public void RefarenceResolution(Scope scope)
        {
            if (IsPragmaAccess)
            {
                _Reference = Root.GetPragma(Value);
            }
            else
            {
                _Reference = scope.NameResolution(Value);
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
            if (Reference is UndefinedOverLoad)
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
