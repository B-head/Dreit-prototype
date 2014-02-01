using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    class RoutineTranslator : ContextTranslator
    {
        public RoutineTranslator(string name, Translator parent)
            : base(name, parent)
        {

        }

        /*public override void BuildCode()
        {
            base.BuildCode();
            Generator = Builder.GetILGenerator();
            Generator.Emit(OpCodes.Ret);
        }

        public override Translator CreateAttribute(FullName gen, FullName type)
        {
            Type dataType = FindTranslator(type).GetDataType();
            LocalBuilder builder = Generator.DeclareLocal(dataType);
            LocalTranslator result = new LocalTranslator(gen, this, builder);
            _Child.Add(result);
            return result;
        }

        public override void GenelatePrimitive(int value)
        {
            Generator.Emit(OpCodes.Ldc_I4, (int)value);
            Generator.Emit(OpCodes.Newobj, typeof(DlightObject.Integer32).GetConstructor(new Type[] { typeof(int) }));
        }

        public override void GenelatePrimitive(double value)
        {
            Generator.Emit(OpCodes.Ldc_R8, (double)value);
            Generator.Emit(OpCodes.Newobj, typeof(DlightObject.Binary64).GetConstructor(new Type[] { typeof(double) }));
        }

        public override void GenelateOperate(FullName type, TokenType operation)
        {
            Type dataType = FindTranslator(type).GetDataType();
            switch (operation)
            {
                case TokenType.Add: Generator.Emit(OpCodes.Call, dataType.GetMethod("opAdd")); break;
                case TokenType.Subtract: Generator.Emit(OpCodes.Call, dataType.GetMethod("opSubtract")); break;
                case TokenType.Multiply: Generator.Emit(OpCodes.Call, dataType.GetMethod("opMultiply")); break;
                case TokenType.Divide: Generator.Emit(OpCodes.Call, dataType.GetMethod("opDivide")); break;
                case TokenType.Modulo: Generator.Emit(OpCodes.Call, dataType.GetMethod("opModulo")); break;
                case TokenType.Special: Generator.EmitWriteLine(FindTranslator(type).GetLocal()); break;
                default: throw new ArgumentException();
            }
        }

        public override void GenelateLoad(FullName fullname)
        {
            LocalBuilder local = FindTranslator(fullname).GetLocal();
            Generator.Emit(OpCodes.Ldloc, local);
        }

        public override void GenelateStore(FullName fullname = null)
        {
            if(fullname == null)
            {
                Generator.Emit(OpCodes.Pop);
                return;
            }
            LocalBuilder local = FindTranslator(fullname).GetLocal();
            Generator.Emit(OpCodes.Stloc, local);
        }*/
    }
}
