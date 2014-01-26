using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class Identifier : Reference
    {
        public string Value { get; set; }

        public override string ElementInfo()
        {
            return base.ElementInfo() + Value;
        }

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            Scope<Element> temp = scope.NameResolution(Value);
            if (temp == null)
            {
                manager.Error(ErrorInfo() + "このスコープで識別子 " + Value + " が宣言されていません。");
                return;
            }
            base.CheckSemantic(manager, temp);
        }

        public override void Translate(Translator trans, Scope<Element> scope, bool assign)
        {
            Scope<Element> temp = scope.NameResolution(Value);
            if (assign)
            {
                trans.GenelateStore(temp.GetFullName());
            }
            else
            {
                trans.GenelateLoad(temp.GetFullName());
            }
            base.Translate(trans, temp, assign);
        }
    }

    class DeclareVariable : Reference
    {
        public Identifier Name { get; set; }
        public Identifier DataType { get; set; }
        public Scope<Element> Scope { get; set; }

        public override string ElementInfo()
        {
            if (DataType == null)
            {
                return base.ElementInfo() + Name.Value;
            }
            else
            {
                return base.ElementInfo() + Name.Value + ":" + DataType.Value;
            }
        }

        public override void CreateScope(Scope<Element> scope)
        {
            Scope = scope.CreateChild(this, Name.Value);
            base.CreateScope(Scope);
        }

        public override void CheckSemantic(ErrorManager manager, Scope<Element> scope)
        {
            base.CheckSemantic(manager, Scope);
        }

        public override void Translate(Translator trans, Scope<Element> scope, bool assign)
        {
            Translator temp = trans.CreateVariable(Scope, "Integer32");
            Name.Translate(temp, Scope, assign);
        }
    }
}
