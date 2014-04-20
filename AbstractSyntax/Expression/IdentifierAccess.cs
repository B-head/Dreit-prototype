using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax.Daclate;

namespace AbstractSyntax.Expression
{
    public class IdentifierAccess : Element
    {
        public string Value { get; set; }
        public bool IsPragmaAccess { get; set; }
        public bool IsTacitThis { get; private set; }
        private OverLoadScope _Reference;

        public IdentifierAccess()
        {
            _Reference = new OverLoadScope();
        }

        public override DataType DataType
        {
            get { return _Reference.GetDataType(); }
        }

        public override OverLoadScope Reference
        {
            get { return _Reference; }
        }

        public override bool IsAssignable
        {
            get { return true; }
        }

        protected override string AdditionalInfo()
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
                var temp = Root.GetPragma(Value);
                if (temp == null)
                {
                    CompileError("プラグマ @@" + Value + " は定義されていません。");
                }
                else
                {
                    _Reference = temp;
                }
            }
            else
            {
                var temp = scope.NameResolution(Value);
                if (temp == null)
                {
                    CompileError("識別子 " + Value + " は宣言されていません。");
                }
                else
                {
                    _Reference = temp;
                }
            }
            /*if (ScopeParent is DeclateRoutine && _Reference.ScopeParent == GetParentClass())
            {
                IsTacitThis = true;
            }*/
        }

        private Scope GetParentClass()
        {
            var current = ScopeParent;
            while(current != null)
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
