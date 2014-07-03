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
                if (!HasThisMember(CallScope))
                {
                    _IsTacitThis = false;
                }
                else if (!(CurrentScope is DeclateRoutine)) //todo この条件が必要な理由を調べる。（忘れたｗ）
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
                var c = GetParent<ClassSymbol>();
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

        private bool IsStaticLocation()
        {
            var r = GetParent<RoutineSymbol>();
            if (r == null)
            {
                return false;
            }
            return r.IsStaticMember;
        }

        private bool HasThisMember(Scope scope)
        {
            if (CallScope.IsStaticMember || CallScope is ThisSymbol)
            {
                return false;
            }
            var cls = GetParent<ClassSymbol>();
            while (cls != null)
            {
                if (scope.CurrentScope == cls)
                {
                    return true;
                }
                cls = cls.InheritClass;
            }
            return false;
        }

        internal override void CheckSemantic()
        {
            base.CheckSemantic();
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
            var s = CallScope;
            if(s.IsAnyAttribute(AttributeType.Private) && !HasCurrentAccess(s.CurrentScope))
            {
                CompileError("not-accessable");
            }
            if(s.IsInstanceMember && IsStaticLocation() && !(Parent is Postfix)) //todo Postfixだけではなく包括的な例外処理をする。
            {
                CompileError("not-accessable");
            }
        }
    }
}
