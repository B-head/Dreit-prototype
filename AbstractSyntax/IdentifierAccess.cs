using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class IdentifierAccess : Element
    {
        public string Value { get; set; }
        public bool IsPragmaAccess { get; set; }
        public bool IsTacitThis { get; private set; } 
        public Scope Refer { get; private set; }

        public IdentifierAccess()
        {
            Refer = new VoidScope();
        }

        public override Scope DataType
        {
            get 
            { 
                return Refer.DataType; 
            }
        }

        public override Scope Reference
        {
            get { return Refer; }
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
                    Refer = temp;
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
                    Refer = temp;
                }
            }
            if (ScopeParent is DeclateRoutine && Refer.ScopeParent == GetParentClass())
            {
                IsTacitThis = true;
            }
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
