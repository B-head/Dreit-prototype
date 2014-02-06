using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CilTranslate
{
    public abstract partial class ContextTranslator : Translator
    {
        private List<VirtualCode> _Code;
        public IReadOnlyList<VirtualCode> Code { get { return _Code; } }

        protected ContextTranslator(string name, Translator parent)
            : base(name, parent)
        {
            _Code = new List<VirtualCode>();
        }

        private void AppendCode(VirtualCodeType type, object operand = null)
        {
            var code = new VirtualCode { Type = type, Operand = operand };
            _Code.Add(code);
        }

        public override Translator CreateClass(string name)
        {
            return new ClassTranslator(name, this);
        }

        public override Translator CreateEnum(string name)
        {
            return new EnumTranslator(name, this);
        }

        public override Translator CreatePoly(string name)
        {
            return new PolyTranslator(name, this);
        }

        public override Translator CreateRoutine(string name)
        {
            return new RoutineTranslator(name, this);
        }

        public override Translator CreateOperation(VirtualCodeType operation)
        {
            return new OperationTranslator(operation, this);
        }

        public override Translator CreateVariant(string name)
        {
            return new VariantTranslator(name, this);
        }

        public override Translator CreateLabel(string name)
        {
            return new LabelTranslator(name, this);
        }

        public override void GenelateControl(VirtualCodeType type)
        {
            AppendCode(type);
        }

        public override void GenelatePrimitive(object value)
        {
            AppendCode(VirtualCodeType.Push, value);
        }

        public override void GenelateLoad(Translator type)
        {
            AppendCode(VirtualCodeType.Load, type);
        }

        public override void GenelateStore(Translator type)
        {
            AppendCode(VirtualCodeType.Store, type);
        }

        public override void GenelateCall(Translator type)
        {
            AppendCode(VirtualCodeType.Call, type);
        }
    }
}
