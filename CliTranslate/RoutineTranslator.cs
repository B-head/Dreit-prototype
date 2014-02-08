using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Common;

namespace CliTranslate
{
    public class RoutineTranslator : ContextTranslator
    {
        public MethodInfo Method { get; private set; }

        public RoutineTranslator(FullPath path, Translator parent, MethodInfo method = null)
            : base(path, parent)
        {
            Method = method;
        }

        public override Translator CreateGeneric(FullPath path)
        {
            return new GenericTranslator(path, this);
        }

        public override Translator CreateArgument(FullPath path)
        {
            return new ArgumentTranslator(path, this);
        }

        /*public override void BuildCode()
        {
            base.BuildCode();
            Generator = Builder.GetILGenerator();
            Generator.Emit(OpCodes.Ret);
        }

        public override Translator CreateRoutine(FullName gen)
        {
            MethodAttributes attr = MethodAttributes.Static;
            MethodBuilder builder = Builder.DefineGlobalMethod(gen.Name, attr, null, null);
            RoutineTranslator result = new RoutineTranslator(gen, this, builder);
            Child.Add(result);
            return result;
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
