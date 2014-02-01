using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.CilTranslate
{
    abstract class ContextTranslator : Translator
    {
        private List<VirtualCode> Code;

        protected ContextTranslator(string name, Translator parent)
            : base(name, parent)
        {
            Code = new List<VirtualCode>();
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

        public override Translator CreateGeneric(string name)
        {
            return new GenericTranslator(name, this);
        }

        public override Translator CreateRoutine(string name)
        {
            return new RoutineTranslator(name, this);
        }

        public override Translator CreateOperation(TokenType operation)
        {
            return new OperationTranslator(operation, this);
        }

        public override Translator CreateVariant(string name)
        {
            return new VariantTranslator(name, this);
        }

        public override Translator CreateAttribute(string name)
        {
            return new AttributeTranslator(name, this);
        }

        public override Translator CreateLabel(string name)
        {
            return new LabelTranslator(name, this);
        }

        public override void GenelatePrimitive(object value)
        {

        }

        public override void GenelateControl(VirtualCodeType type)
        {

        }

        public override void GenelateLoad(Translator type)
        {

        }

        public override void GenelateStore(Translator type)
        {

        }

        public override void GenelateCall(Translator type)
        {

        }
    }
}
