using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CliTranslate
{
    public abstract partial class ContextTranslator : Translator
    {
        private List<VirtualCode> _Code;
        public IReadOnlyList<VirtualCode> Code { get { return _Code; } }

        protected ContextTranslator(FullPath path, Translator parent)
            : base(path, parent)
        {
            _Code = new List<VirtualCode>();
        }

        private void AppendCode(VirtualCodeType type, object operand = null)
        {
            var code = new VirtualCode { Type = type, Operand = operand };
            _Code.Add(code);
        }

        public override Translator CreateClass(FullPath path)
        {
            return new ClassTranslator(path, this);
        }

        public override Translator CreateEnum(FullPath path)
        {
            return new EnumTranslator(path, this);
        }

        public override Translator CreatePoly(FullPath path)
        {
            return new PolyTranslator(path, this);
        }

        public override Translator CreateRoutine(FullPath path)
        {
            return new RoutineTranslator(path, this);
        }

        public override Translator CreateOperation(VirtualCodeType operation)
        {
            return new OperationTranslator(operation, this);
        }

        public override Translator CreateVariant(FullPath path)
        {
            return new VariantTranslator(path, this);
        }

        public override Translator CreateLabel(FullPath path)
        {
            return new LabelTranslator(path, this);
        }

        public override void GenelateControl(VirtualCodeType type)
        {
            AppendCode(type);
        }

        public override void GenelatePrimitive(object value)
        {
            AppendCode(VirtualCodeType.Push, value);
        }

        public override void GenelateLoad(FullPath type)
        {
            AppendCode(VirtualCodeType.Load, type);
        }

        public override void GenelateStore(FullPath type)
        {
            AppendCode(VirtualCodeType.Store, type);
        }

        public override void GenelateCall(FullPath type)
        {
            AppendCode(VirtualCodeType.Call, type);
        }
    }
}
