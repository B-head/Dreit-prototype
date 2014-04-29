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
        public bool IsTacitThis { get; private set; }
        private OverLoadScope _Reference;

        public override DataType DataType
        {
            get { return _Reference.GetDataType(); }
        }

        public OverLoadScope Reference
        {
            get { return _Reference; }
        }

        protected override string ElementInfo()
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

        internal override void SpreadReference(Scope scope)
        {
            if (IsPragmaAccess)
            {
                _Reference = Root.GetPragma(Value);
                if (_Reference.IsVoid)
                {
                    CompileError("undefined-pragma");
                    return;
                }
            }
            else
            {
                _Reference = scope.NameResolution(Value);
                if (_Reference.IsVoid)
                {
                    CompileError("undefined-identifier");
                    return;
                }
            }
            var voidSelect = _Reference.TypeSelect();
            if (ScopeParent is DeclateRoutine && voidSelect != null && voidSelect.ScopeParent == GetParentClass())
            {
                IsTacitThis = true;
            }
        }

        private Scope GetParentClass()
        {
            var current = ScopeParent;
            while(current != null) //todo バグ潰してる？
            {
                if(current is DeclateClass)
                {
                    break;
                }
                current = current.ScopeParent;
            }
            return current;
        }
    }
}
