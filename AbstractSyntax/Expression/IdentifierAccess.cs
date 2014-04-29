using AbstractSyntax.Daclate;
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
        private OverLoadScope _Reference;

        public bool IsTacitThis
        {
            get
            {
                if (_IsTacitThis == null)
                {
                    var voidSelect = ScopeReference;
                    if (voidSelect != null && voidSelect.CurrentScope == GetParentClass() && CurrentScope is DeclateRoutine)
                    {
                        _IsTacitThis = true;
                    }
                    else
                    {
                        _IsTacitThis = false;
                    }
                }
                return _IsTacitThis.Value;
            }
        }

        public override DataType DataType
        {
            get
            {
                if (_Reference.IsVoid == true)
                {
                    GetReference(CurrentScope);
                }
                return _Reference.GetDataType(); 
            }
        }

        public OverLoadScope Reference
        {
            get
            {
                if(_Reference.IsVoid == true)
                {
                    GetReference(CurrentScope);
                }
                return _Reference;
            }
        }

        public Scope ScopeReference
        {
            get
            {
                if (_Reference.IsVoid == true)
                {
                    GetReference(CurrentScope);
                }
                return _Reference.TypeSelect();
            }
        }

        public OverLoadScope GetReference(Scope scope)
        {
            if (IsPragmaAccess)
            {
                _Reference = Root.GetPragma(Value);
                if (_Reference.IsVoid)
                {
                    CompileError("undefined-pragma");
                }
            }
            else
            {
                _Reference = scope.NameResolution(Value);
                if (_Reference.IsVoid)
                {
                    CompileError("undefined-identifier");
                }
            }
            return _Reference;
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

        internal override void SpreadReference(Scope scope)
        {
            _Reference = Root.VoidOverLoad;
            base.SpreadReference(scope);
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
    }
}
