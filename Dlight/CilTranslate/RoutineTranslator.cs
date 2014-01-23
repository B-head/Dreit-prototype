using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class RoutineTranslator : CilTranslator
    {
        private MethodBuilder Builder { get; set; }
        private ILGenerator Generator { get; set; }

        public RoutineTranslator(string name, CilTranslator parent, MethodBuilder builder)
            : base(name, parent)
        {
            Builder = builder;
            Generator = Builder.GetILGenerator();
        }

        public RoutineTranslator(Scope<Element> scope, CilTranslator parent, MethodBuilder builder)
            : base(scope, parent)
        {
            Builder = builder;
            Generator = Builder.GetILGenerator();
        }

        public override MethodInfo GetContext()
        {
            return Builder;
        }

        public override Type GetDataType()
        {
            return Builder.DeclaringType;
        }

        public override void Save()
        {
            base.Save();
            Generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(object) }));
            Generator.Emit(OpCodes.Ret);
        }

        public override void GenelateNumber(int value)
        {
            Generator.Emit(OpCodes.Ldc_I4, (int)value);
            Generator.Emit(OpCodes.Newobj, typeof(DlightObject.Integer32).GetConstructor(new Type[] { typeof(int) }));
        }

        public override void GenelateBinomial(Type dataType, SyntaxType operation)
        {
            switch (operation)
            {
                case SyntaxType.Add: Generator.Emit(OpCodes.Call, dataType.GetMethod("opAdd")); break;
                case SyntaxType.Subtract: Generator.Emit(OpCodes.Call, dataType.GetMethod("opSubtract")); break;
                case SyntaxType.Combine: Generator.Emit(OpCodes.Call, dataType.GetMethod("opCombine")); break;
                case SyntaxType.Multiply: Generator.Emit(OpCodes.Call, dataType.GetMethod("opMultiply")); break;
                case SyntaxType.Divide: Generator.Emit(OpCodes.Call, dataType.GetMethod("opDivide")); break;
                case SyntaxType.Modulo: Generator.Emit(OpCodes.Call, dataType.GetMethod("opModulo")); break;
                case SyntaxType.Exponent: Generator.Emit(OpCodes.Call, dataType.GetMethod("opExponent")); break;
                case SyntaxType.Or: Generator.Emit(OpCodes.Call, dataType.GetMethod("opOr")); break;
                case SyntaxType.And: Generator.Emit(OpCodes.Call, dataType.GetMethod("opAnd")); break;
                case SyntaxType.Xor: Generator.Emit(OpCodes.Call, dataType.GetMethod("opXor")); break;
                case SyntaxType.LeftShift: Generator.Emit(OpCodes.Call, dataType.GetMethod("opLeftShift")); break;
                case SyntaxType.RightShift: Generator.Emit(OpCodes.Call, dataType.GetMethod("opRightShift")); break;
                //case SyntaxType.ArithRightShift: Generator.Emit(OpCodes.Call, dataType.GetMethod("opArithRightShift")); break;
                //case SyntaxType.LeftRotate: Generator.Emit(OpCodes.Call, dataType.GetMethod("opLeftRotate")); break;
                //case SyntaxType.RightRotate: Generator.Emit(OpCodes.Call, dataType.GetMethod("opRightRotate")); break;
                default: throw new ArgumentException();
            }
        }
    }
}
