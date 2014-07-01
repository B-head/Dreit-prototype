using AbstractSyntax.Daclate;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            get { return CallScope.ReturnType; }
        }

        public Scope CallScope
        {
            get { return Reference.CallSelect().Call; }
        }

        public OverLoad Reference
        {
            get
            {
                if(_Reference == null)
                {
                    RefarenceResolution();
                }
                return _Reference;
            }
        }

        public void RefarenceResolution()
        {
            var p = Parent as IAccess;
            if (p == null)
            {
                RefarenceResolution(CurrentIScope);
            }
            else
            {
                p.RefarenceResolution();
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

        private bool HasCurrentAccess(IScope other)
        {
            var c = CurrentIScope;
            while(c != null)
            {
                if(c == other)
                {
                    return true;
                }
                c = c.CurrentIScope;
            }
            return false;
        }

        internal override void CheckSyntax()
        {
            base.CheckSyntax();
            var s = Reference.CallSelect().Call;
            if(s.IsAnyAttribute(AttributeType.Private) && !HasCurrentAccess(s.CurrentIScope))
            {
                CompileError("not-accessable");
            }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            if (Reference.IsUndefined)
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
        }
    }
}
