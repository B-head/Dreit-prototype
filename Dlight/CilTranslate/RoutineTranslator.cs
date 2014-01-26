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

        public override Translator CreateVariable(Scope<Element> scope, string fullName)
        {
            Type dataType = FindTranslator(fullName).GetDataType();
            LocalBuilder builder = Generator.DeclareLocal(dataType);
            LocalTranslator result = new LocalTranslator(scope, this, builder);
            Child.Add(result);
            return result;
        }

        public override void GenelateLoad(string fullName)
        {
            LocalBuilder local = FindTranslator(fullName).GetLocal();
            Generator.Emit(OpCodes.Ldloc, local);
        }

        public override void GenelateStore(string fullName)
        {
            LocalBuilder local = FindTranslator(fullName).GetLocal();
            Generator.Emit(OpCodes.Stloc, local);
        }

        public override void GenelateNumber(int value)
        {
            Generator.Emit(OpCodes.Ldc_I4, (int)value);
            Generator.Emit(OpCodes.Newobj, typeof(DlightObject.Integer32).GetConstructor(new Type[] { typeof(int) }));
        }

        public override void GenelateBinomial(string fullName, TokenType operation)
        {
            Type dataType = FindTranslator(fullName).GetDataType();
            switch (operation)
            {
                case TokenType.Add: Generator.Emit(OpCodes.Call, dataType.GetMethod("opAdd")); break;
                case TokenType.Subtract: Generator.Emit(OpCodes.Call, dataType.GetMethod("opSubtract")); break;
                case TokenType.Multiply: Generator.Emit(OpCodes.Call, dataType.GetMethod("opMultiply")); break;
                case TokenType.Divide: Generator.Emit(OpCodes.Call, dataType.GetMethod("opDivide")); break;
                case TokenType.Modulo: Generator.Emit(OpCodes.Call, dataType.GetMethod("opModulo")); break;
                default: throw new ArgumentException();
            }
        }
    }
}
