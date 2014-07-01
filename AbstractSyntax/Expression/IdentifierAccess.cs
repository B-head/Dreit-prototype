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
    public class IdentifierAccess : Element
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

        public override IDataType ReturnType
        {
            get { return CallScope.CallReturnType; }
        }

        public Scope CallScope
        {
            get { return Reference.CallSelect().Call; }
        }

        public override OverLoad Reference
        {
            get
            {
                if(_Reference != null)
                {
                    return _Reference;
                }
                if (IsPragmaAccess)
                {
                    _Reference = CurrentScope.NameResolution("@@" + Value);
                }
                else
                {
                    _Reference = CurrentScope.NameResolution(Value);
                }
                return _Reference;
            }
        }

        private Scope GetParentClass()
        {
            var current = CurrentScope;
            while(current != null)
            {
                if(current is ClassSymbol)
                {
                    break;
                }
                current = current.CurrentScope;
            }
            return current;
        }

        private Scope GetParentRoutine()
        {
            var current = CurrentScope;
            while (current != null)
            {
                if (current is RoutineSymbol)
                {
                    break;
                }
                current = current.CurrentScope;
            }
            return current;
        }

        private bool IsStaticAccess()
        {
            var r = GetParentRoutine();
            if (r == null)
            {
                return false;
            }
            return r.IsStaticMember;
        }

        internal override void CheckSyntax()
        {
            base.CheckSyntax();
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
            var s = Reference.CallSelect().Call;
            if(s.IsAnyAttribute(AttributeType.Private) && !HasCurrentAccess(s.CurrentScope))
            {
                CompileError("not-accessable");
            }
            if(s.IsInstanceMember && IsStaticAccess())
            {
                CompileError("not-accessable");
            }
        }
    }
}
