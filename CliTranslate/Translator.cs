using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Daclate;

namespace CliTranslate
{
    public abstract class Translator
    {
        public RootTranslator Root { get; private set; }
        public Translator Parent { get; private set; }
        private List<Translator> _Child;
        public IReadOnlyList<Translator> Child { get { return _Child; } }
        public Scope Scope { get; private set; }
        protected ILGenerator Generator;

        protected Translator(Scope scope, Translator parent)
        {
            Scope = scope;
            _Child = new List<Translator>();
            if(parent == null)
            {
                Root = (RootTranslator)this;
            }
            else
            {
                Root = parent.Root;
                parent.AddChild(this);
            }
        }

        private void AddChild(Translator child)
        {
            _Child.Add(child);
            child.Parent = this;
        }

        public virtual void BuildCode()
        {
            foreach (var v in _Child)
            {
                v.BuildCode();
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + ": " + Scope.GetFullName();
        }

        public virtual bool IsThisArg
        {
            get { return false; }
        }

        internal virtual TypeBuilder CreateLexical(string name)
        {
            throw new NotSupportedException();
        }

        public virtual ModuleTranslator CreateModule(Scope path)
        {
            throw new NotSupportedException();
        }

        public virtual RoutineTranslator CreateRoutine(Scope path, Scope returnType, IEnumerable<Scope> argumentType)
        {
            throw new NotSupportedException();
        }

        public virtual RoutineTranslator CreateOperation(OpCodes operation)
        {
            throw new NotSupportedException();
        }

        public virtual ClassTranslator CreateClass(Scope path)
        {
            throw new NotSupportedException();
        }

        public virtual PrimitiveTranslator CreatePrimitive(Scope path, PrimitivePragmaType type)
        {
            throw new NotSupportedException();
        }

        public virtual void CreateEnum(Scope path)
        {
            throw new NotSupportedException();
        }

        public virtual void CreateVariant(Scope path, Scope type)
        {
            throw new NotSupportedException();
        }

        public void CreateLabel(Scope path)
        {
            var builder = Generator.DefineLabel();
            Root.RegisterBuilder(path, builder);
        }

        internal void BuildInitCall(MethodBuilder method)
        {
            Generator.Emit(OpCodes.Call, method);
        }

        public void GenerateControl(OpCode type)
        {
            Generator.Emit(type);
        }

        public void GeneratePrimitive(int value)
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

        public void GeneratePrimitive(long value)
        {
            Generator.Emit(OpCodes.Ldc_I8, value);
        }

        public void GeneratePrimitive(float value)
        {
            Generator.Emit(OpCodes.Ldc_R4, value);
        }

        public void GeneratePrimitive(double value)
        {
            Generator.Emit(OpCodes.Ldc_R8, value);
        }

        public void GeneratePrimitive(string value)
        {
            Generator.Emit(OpCodes.Ldstr, value);
        }

        public virtual void GenerateLoad(Scope name, bool address = false)
        {
            if(name is ThisSymbol)
            {
                GenerateLoad((ThisSymbol)name, address);
                return;
            }
            dynamic temp = Root.GetBuilder(name);
            BuildLoad(temp, address);
        }

        protected void GenerateLoad(ThisSymbol name, bool address)
        {
            var c = name.DataType as DeclateClass;
            var pe = c.GetPrimitiveType();
            Generator.Emit(OpCodes.Ldarg_0);
            return;
            /*
            if (pe == PrimitivePragmaType.NotPrimitive)
            {
                Generator.Emit(OpCodes.Ldarg_0);
                return;
            }
            var pt = PrimitiveTranslator.GetPrimitiveType(pe);
            Generator.Emit(OpCodes.Ldarg_0);
            BuildLoad(pt, address);
             */
        }

        protected void BuildLoad(Type type, bool address)
        {
            Generator.Emit(OpCodes.Ldobj, type);
        }

        protected void BuildLoad(LocalBuilder local, bool address)
        {
            if (address)
            {
                if (local.LocalIndex <= 255)
                {
                    Generator.Emit(OpCodes.Ldloca_S, local);
                }
                else
                {
                    Generator.Emit(OpCodes.Ldloca, local);
                }
            }
            else
            {
                if (local.LocalIndex <= 255)
                {
                    switch (local.LocalIndex)
                    {
                        case 0: Generator.Emit(OpCodes.Ldloc_0); break;
                        case 1: Generator.Emit(OpCodes.Ldloc_1); break;
                        case 2: Generator.Emit(OpCodes.Ldloc_2); break;
                        case 3: Generator.Emit(OpCodes.Ldloc_3); break;
                        default: Generator.Emit(OpCodes.Ldloc_S, local); break;
                    }
                }
                else
                {
                    Generator.Emit(OpCodes.Ldloc, local);
                }
            }
        }

        protected void BuildLoad(FieldBuilder field, bool address)
        {
            if (field.IsStatic)
            {
                if (address)
                {
                    Generator.Emit(OpCodes.Ldsflda, field);
                }
                else
                {
                    Generator.Emit(OpCodes.Ldsfld, field);
                }
            }
            else
            {
                if (address)
                {
                    Generator.Emit(OpCodes.Ldflda, field);
                }
                else
                {
                    Generator.Emit(OpCodes.Ldfld, field);
                }
            }
        }

        protected void BuildLoad(ParameterBuilder param, bool address)
        {
            int index = IsThisArg ? param.Position : param.Position - 1;
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

        public virtual void GenerateStore(Scope name, bool address = false)
        {
            if (name is ThisSymbol)
            {
                GenerateStore((ThisSymbol)name);
                return;
            }
            dynamic temp = Root.GetBuilder(name);
            BuildStore(temp, address);
        }

        protected void GenerateStore(ThisSymbol name, bool address)
        {
            var c = name.DataType as DeclateClass;
            var pe = c.GetPrimitiveType();
            Generator.Emit(OpCodes.Starg_S, 0);
            return;
            /*
            if (pe == PrimitivePragmaType.NotPrimitive)
            {
                Generator.Emit(OpCodes.Starg_S, 0);
                return;
            }
            var pt = PrimitiveTranslator.GetPrimitiveType(pe);
            var local = Generator.DeclareLocal(pt); //todo 後で整理する。
            BuildStore(local, false);
            Generator.Emit(OpCodes.Ldarg_0); 
            BuildLoad(local, false);
            BuildStore(pt, address);
             */
        }

        protected void BuildStore(Type type, bool address)
        {
            Generator.Emit(OpCodes.Stobj, type);
        }

        protected void BuildStore(LocalBuilder local, bool address)
        {
            if (local.LocalIndex <= 255)
            {
                switch (local.LocalIndex)
                {
                    case 0: Generator.Emit(OpCodes.Stloc_0); break;
                    case 1: Generator.Emit(OpCodes.Stloc_1); break;
                    case 2: Generator.Emit(OpCodes.Stloc_2); break;
                    case 3: Generator.Emit(OpCodes.Stloc_3); break;
                    default: Generator.Emit(OpCodes.Stloc_S, local); break;
                }
            }
            else
            {
                Generator.Emit(OpCodes.Stloc, local);
            }
        }

        protected void BuildStore(FieldBuilder field, bool address)
        {
            if (field.IsStatic)
            {
                Generator.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                Generator.Emit(OpCodes.Stfld, field);
            }
        }

        protected void BuildStore(ParameterBuilder param, bool address)
        {
            int index = IsThisArg ? param.Position : param.Position - 1;
            if (index <= 255)
            {
                Generator.Emit(OpCodes.Starg_S, index);
            }
            else
            {
                Generator.Emit(OpCodes.Starg, index);
            }
        }

        public virtual void GenerateCall(Scope name)
        {
            var temp = Root.GetBuilder(name);
            if (temp is ConstructorInfo)
            {
                Generator.Emit(OpCodes.Newobj, temp);
            }
            else
            {
                Generator.Emit(OpCodes.Call, temp);
            }
        }

        public void GenerateEcho(Scope type)
        {
            var temp = Root.GetBuilder(type) as Type;
            var types = new Type[] { temp };
            Generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", types));
        }
    }
}
