using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class CodeGenerator
    {
        [NonSerialized]
        private ILGenerator Generator;

        internal CodeGenerator(ILGenerator gen)
        {
            Generator = gen;
        }

        internal LocalBuilder CreateLocal(TypeStructure dt)
        {
            return Generator.DeclareLocal(dt.GainType());
        }

        internal Label CreateLabel()
        {
            return Generator.DefineLabel();
        }

        internal void MarkLabel(LabelStructure label)
        {
            Generator.MarkLabel(label.GainLabel());
        }

        internal void BeginScope()
        {
            Generator.BeginScope();
        }

        internal void EndScope()
        {
            Generator.EndScope();
        }

        internal void GenerateJump(OpCode type, LabelStructure label)
        {
            Generator.Emit(type, label.GainLabel());
        }

        internal void GenerateControl(OpCode type)
        {
            Generator.Emit(type);
        }

        internal void GenerateArray(int length, TypeStructure type)
        {
            GeneratePrimitive(length);
            Generator.Emit(OpCodes.Newarr, type.GainType());
        }

        internal void GenerateNew(ConstructorStructure constructor)
        {
            Generator.Emit(OpCodes.Newobj, constructor.GainConstructor());
        }

        internal void GenerateCall(ConstructorStructure constructor)
        {
            Generator.Emit(OpCodes.Call, constructor.GainConstructor());
        }

        internal void GenerateCall(MethodStructure method)
        {
            if (method.IsVirtual)
            {
                Generator.Emit(OpCodes.Callvirt, method.GainMethod());
            }
            else
            {
                Generator.Emit(OpCodes.Call, method.GainMethod());
            }
        }

        internal void GenerateString(IReadOnlyList<CilStructure> texts)
        {
            var sbc = typeof(StringBuilder).GetConstructor(Type.EmptyTypes);
            var sba = typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) });
            var sbts = typeof(StringBuilder).GetMethod("ToString", Type.EmptyTypes);
            Generator.Emit(OpCodes.Newobj, sbc);
            foreach(var v in texts)
            {
                var exp = (ExpressionStructure)v;
                exp.BuildCode();
                if (exp.ResultType.GainType() != typeof(String))
                {
                    if (exp.ResultType.IsValueType)
                    {
                        Generator.Emit(OpCodes.Box, exp.ResultType.GainType());
                    }
                    var ts = typeof(object).GetMethod("ToString", Type.EmptyTypes);
                    Generator.Emit(OpCodes.Callvirt, ts);
                }
                Generator.Emit(OpCodes.Call, sba);
            }
            Generator.Emit(OpCodes.Call, sbts);
        }

        internal void GenerateWriteLine(ExpressionStructure exp)
        {
            var temp = exp.ResultType.GainType();
            var types = new Type[] { temp };
            var wl = typeof(Console).GetMethod("WriteLine", types);
            exp.BuildCode();
            Generator.Emit(OpCodes.Call, wl);
        }

        internal void GenerateBoxing(TypeStructure from, TypeStructure to)
        {
            if(from.IsValueType && to.IsReferType)
            {
                Generator.Emit(OpCodes.Box, from.GainType());
            }
            else if(from.IsReferType && to.IsValueType)
            {
                Generator.Emit(OpCodes.Unbox, to.GainType());
            }
        }

        internal void GenerateLoad(LocalStructure local)
        {
            var lb = local.GainLocal();
            if (lb.LocalIndex <= 255)
            {
                switch (lb.LocalIndex)
                {
                    case 0: Generator.Emit(OpCodes.Ldloc_0); break;
                    case 1: Generator.Emit(OpCodes.Ldloc_1); break;
                    case 2: Generator.Emit(OpCodes.Ldloc_2); break;
                    case 3: Generator.Emit(OpCodes.Ldloc_3); break;
                    default: Generator.Emit(OpCodes.Ldloc_S, lb); break;
                }
            }
            else
            {
                Generator.Emit(OpCodes.Ldloc, lb);
            }
        }

        internal void GenerateLoad(FieldStructure field)
        {
            var fb = field.GainField();
            if (fb.IsStatic)
            {
                Generator.Emit(OpCodes.Ldsfld, fb);
            }
            else
            {
                Generator.Emit(OpCodes.Ldfld, fb);
            }
        }

        internal void GenerateLoad(ParameterStructure param)
        {
            int index = param.GainPosition();
            if (index <= 255)
            {
                switch (index)
                {
                    case 0: Generator.Emit(OpCodes.Ldarg_0); break;
                    case 1: Generator.Emit(OpCodes.Ldarg_1); break;
                    case 2: Generator.Emit(OpCodes.Ldarg_2); break;
                    case 3: Generator.Emit(OpCodes.Ldarg_3); break;
                    default: Generator.Emit(OpCodes.Ldarg_S, index); break;
                }
            }
            else
            {
                Generator.Emit(OpCodes.Ldarg, index);
            }
        }

        internal void GenerateLoadAddress(LocalStructure local)
        {
            var lb = local.GainLocal();
            if (lb.LocalIndex <= 255)
            {
                Generator.Emit(OpCodes.Ldloca_S, lb);
            }
            else
            {
                Generator.Emit(OpCodes.Ldloca, lb);
            }
        }

        internal void GenerateLoadAddress(FieldStructure field)
        {
            var fb = field.GainField();
            if (fb.IsStatic)
            {
                Generator.Emit(OpCodes.Ldsflda, fb);
            }
            else
            {
                Generator.Emit(OpCodes.Ldflda, fb);
            }
        }

        internal void GenerateLoadAddress(ParameterStructure param)
        {
            int index = param.GainPosition();
            if (index <= 255)
            {
                Generator.Emit(OpCodes.Ldarga_S, index);
            }
            else
            {
                Generator.Emit(OpCodes.Ldarga, index);
            }
        }

        internal void GenerateStore(LocalStructure local)
        {
            var lb = local.GainLocal();
            if (lb.LocalIndex <= 255)
            {
                switch (lb.LocalIndex)
                {
                    case 0: Generator.Emit(OpCodes.Stloc_0); break;
                    case 1: Generator.Emit(OpCodes.Stloc_1); break;
                    case 2: Generator.Emit(OpCodes.Stloc_2); break;
                    case 3: Generator.Emit(OpCodes.Stloc_3); break;
                    default: Generator.Emit(OpCodes.Stloc_S, lb); break;
                }
            }
            else
            {
                Generator.Emit(OpCodes.Stloc, lb);
            }
        }

        internal void GenerateStore(FieldStructure field)
        {
            var fb = field.GainField();
            if (fb.IsStatic)
            {
                Generator.Emit(OpCodes.Stsfld, fb);
            }
            else
            {
                Generator.Emit(OpCodes.Stfld, fb);
            }
        }

        internal void GenerateStore(ParameterStructure param)
        {
            int index = param.GainPosition();
            if (index <= 255)
            {
                Generator.Emit(OpCodes.Starg_S, index);
            }
            else
            {
                Generator.Emit(OpCodes.Starg, index);
            }
        }

        internal void GenerateStoreElement(TypeStructure type)
        {
            var t = type.GainType();
            if(t.IsClass || t.IsInterface)
            {
                Generator.Emit(OpCodes.Stelem_Ref);
            }
            else if (t == typeof(IntPtr)) Generator.Emit(OpCodes.Stelem_I);
            else if (t == typeof(SByte)) Generator.Emit(OpCodes.Stelem_I1);
            else if (t == typeof(Int16)) Generator.Emit(OpCodes.Stelem_I2);
            else if (t == typeof(Int32)) Generator.Emit(OpCodes.Stelem_I4);
            else if (t == typeof(Int64)) Generator.Emit(OpCodes.Stelem_I8);
            else if (t == typeof(Single)) Generator.Emit(OpCodes.Stelem_R4);
            else if (t == typeof(Double)) Generator.Emit(OpCodes.Stelem_R8);
            else Generator.Emit(OpCodes.Stelem);
        }

        internal void GeneratePrimitive(int value)
        {
            if (value <= 127 && value >= -128)
            {
                switch (value)
                {
                    case 0: Generator.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: Generator.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: Generator.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: Generator.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: Generator.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: Generator.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: Generator.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: Generator.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: Generator.Emit(OpCodes.Ldc_I4_8); break;
                    case -1: Generator.Emit(OpCodes.Ldc_I4_M1); break;
                    default: Generator.Emit(OpCodes.Ldc_I4_S, (byte)value); break;
                }
            }
            else
            {
                Generator.Emit(OpCodes.Ldc_I4, value);
            }
        }

        internal void GeneratePrimitive(long value)
        {
            Generator.Emit(OpCodes.Ldc_I8, value);
        }

        internal void GeneratePrimitive(float value)
        {
            Generator.Emit(OpCodes.Ldc_R4, value);
        }

        internal void GeneratePrimitive(double value)
        {
            Generator.Emit(OpCodes.Ldc_R8, value);
        }

        internal void GeneratePrimitive(string value)
        {
            Generator.Emit(OpCodes.Ldstr, value);
        }

        internal void GeneratePrimitive(bool value)
        {
            if (value)
            {
                Generator.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                Generator.Emit(OpCodes.Ldc_I4_0);
            }
        }
    }
}
