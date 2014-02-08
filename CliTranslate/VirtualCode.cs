using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace CliTranslate
{
    public struct VirtualCode
    {
        public VirtualCodeType Type;
        public Object Operand;

        public override string ToString()
        {
            string temp = Operand == null ? "<null>" : Operand.ToString();
            return Enum.GetName(typeof(VirtualCodeType), Type) + ": " + temp;
        }
    }

    public enum VirtualCodeType
    {
        Nop,
        Push,
        Pop,
        Load,
        Store,
        Call,
        Return,
        Add,
        Sub,
        Mul,
        Div,
        Mod,
    }

    abstract partial class ContextTranslator : Translator
    {
        protected void BuildCode(ILGenerator gen)
        {
            foreach(var v in _Code)
            {
                switch(v.Type)
                {
                    case VirtualCodeType.Nop: gen.Emit(OpCodes.Nop); break;
                    case VirtualCodeType.Push: BuildPush(gen, (dynamic)v.Operand); break;
                    case VirtualCodeType.Pop: gen.Emit(OpCodes.Pop); break;
                    case VirtualCodeType.Load: BuildLoad(gen, (dynamic)v.Operand); break;
                    case VirtualCodeType.Store: BuildStore(gen, (dynamic)v.Operand); break;
                    case VirtualCodeType.Call: BuildCall(gen, (dynamic)v.Operand); break;
                    case VirtualCodeType.Return: gen.Emit(OpCodes.Ret); break;
                    case VirtualCodeType.Add: gen.Emit(OpCodes.Add); break;
                    case VirtualCodeType.Sub: gen.Emit(OpCodes.Sub); break;
                    case VirtualCodeType.Mul: gen.Emit(OpCodes.Mul); break;
                    case VirtualCodeType.Div: gen.Emit(OpCodes.Div); break;
                    case VirtualCodeType.Mod: gen.Emit(OpCodes.Rem); break;
                    default: throw new Exception();
                }
            }
        }

        private void BuildPush(ILGenerator gen, int value)
        {
            if(value <= 127 && value >= -128)
            {
                switch(value)
                {
                    case 0: gen.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: gen.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: gen.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: gen.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: gen.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: gen.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: gen.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: gen.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: gen.Emit(OpCodes.Ldc_I4_8); break;
                    case -1: gen.Emit(OpCodes.Ldc_I4_M1); break;
                    default: gen.Emit(OpCodes.Ldc_I4_S, (byte)value); break;
                }
            }
            else
            {
                gen.Emit(OpCodes.Ldc_I4, value);
            }
        }

        private void BuildPush(ILGenerator gen, long value)
        {
            gen.Emit(OpCodes.Ldc_I8, value);
        }

        private void BuildPush(ILGenerator gen, float value)
        {
            gen.Emit(OpCodes.Ldc_R4, value);
        }

        private void BuildPush(ILGenerator gen, double value)
        {
            gen.Emit(OpCodes.Ldc_R8, value);
        }

        private void BuildPush(ILGenerator gen, string value)
        {
            gen.Emit(OpCodes.Ldstr, value);
        }

        private void BuildLoad(ILGenerator gen, ArgumentTranslator trans)
        {

        }

        private void BuildLoad(ILGenerator gen, VariantTranslator trans)
        {
            var local = trans.Local;
            var field = trans.Field;
            if(local != null)
            {
                if(local.LocalIndex <= 255)
                {
                    switch(local.LocalIndex)
                    {
                        case 0: gen.Emit(OpCodes.Ldloc_0); break;
                        case 1: gen.Emit(OpCodes.Ldloc_1); break;
                        case 2: gen.Emit(OpCodes.Ldloc_2); break;
                        case 3: gen.Emit(OpCodes.Ldloc_3); break;
                        default: gen.Emit(OpCodes.Ldloc_S, local); break;
                    }
                }
                else
                {
                    gen.Emit(OpCodes.Ldloc, local);
                }
            }
            else if(field != null)
            {
                if (field.IsStatic)
                {
                    gen.Emit(OpCodes.Ldsfld, field);
                }
                else
                {
                    gen.Emit(OpCodes.Ldfld, field);
                }
            }
            else
            {
                throw new Exception();
            }
        }

        private void BuildStore(ILGenerator gen, ArgumentTranslator trans)
        {

        }

        private void BuildStore(ILGenerator gen, VariantTranslator trans)
        {
            var local = trans.Local;
            var field = trans.Field;
            if (local != null)
            {
                if (local.LocalIndex <= 255)
                {
                    switch (local.LocalIndex)
                    {
                        case 0: gen.Emit(OpCodes.Stloc_0); break;
                        case 1: gen.Emit(OpCodes.Stloc_1); break;
                        case 2: gen.Emit(OpCodes.Stloc_2); break;
                        case 3: gen.Emit(OpCodes.Stloc_3); break;
                        default: gen.Emit(OpCodes.Stloc_S, local); break;
                    }
                }
                else
                {
                    gen.Emit(OpCodes.Stloc, local);
                }
            }
            else if (field != null)
            {
                if (field.IsStatic)
                {
                    gen.Emit(OpCodes.Stsfld, field);
                }
                else
                {
                    gen.Emit(OpCodes.Stfld, field);
                }
            }
            else
            {
                throw new Exception();
            }
        }

        private void BuildCall(ILGenerator gen, RoutineTranslator trans)
        {
            gen.Emit(OpCodes.Call, trans.Method);
        }

        private void BuildCall(ILGenerator gen, ClassTranslator trans)
        {
            gen.Emit(OpCodes.Newobj, trans.TypeInfo.GetConstructors()[0]);//ここ対応不十分。
        }
    }
}
