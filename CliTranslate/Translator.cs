using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Pragma;

namespace CliTranslate
{
    public enum CodeType
    {
        Nop,
        Pop,
        Ret,
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        This,
        Echo,
    }

    public abstract class Translator
    {
        public RootTranslator Root { get; private set; }
        public Translator Parent { get; private set; }
        private List<Translator> _Child;
        public IReadOnlyList<Translator> Child { get { return _Child; } }
        public FullPath Path { get; private set; }
        protected ILGenerator Generator;

        protected Translator(FullPath path, Translator parent)
        {
            Path = path;
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

        public virtual void Save()
        {
            foreach (var v in _Child)
            {
                v.Save();
            }
        }

        private string Indent(int indent)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                result.Append(" ");
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Indent(indent) + this.GetType().Name + ": " + Path.ToString() + "(" + Path.Id + ")");
            foreach (var v in _Child)
            {
                builder.Append(v.ToString(indent + 1));
            }
            return builder.ToString(); 
        }

        internal virtual TypeBuilder CreateLexical(string name)
        {
            throw new NotSupportedException();
        }

        public virtual ModuleTranslator CreateModule(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual RoutineTranslator CreateRoutine(FullPath path, FullPath returnType, FullPath[] argumentType)
        {
            throw new NotSupportedException();
        }

        public virtual RoutineTranslator CreateOperation(CodeType operation)
        {
            throw new NotSupportedException();
        }

        public virtual ClassTranslator CreateClass(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual PrimitiveTranslator CreatePrimitive(FullPath path, PrimitivePragmaType type)
        {
            throw new NotSupportedException();
        }

        public virtual void CreateEnum(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual void CreateVariant(FullPath path, FullPath type)
        {
            throw new NotSupportedException();
        }

        public void CreateLabel(FullPath path)
        {
            var builder = Generator.DefineLabel();
            Root.RegisterBuilder(path, builder);
        }

        internal void BuildInitCall(MethodBuilder method)
        {
            Generator.Emit(OpCodes.Call, method);
        }

        public void GenerateControl(CodeType type)
        {
            switch (type)
            {
                case CodeType.Nop: Generator.Emit(OpCodes.Nop); break;
                case CodeType.Pop: Generator.Emit(OpCodes.Pop); break;
                case CodeType.Ret: Generator.Emit(OpCodes.Ret); break;
                case CodeType.Add: Generator.Emit(OpCodes.Add); break;
                case CodeType.Sub: Generator.Emit(OpCodes.Sub); break;
                case CodeType.Mul: Generator.Emit(OpCodes.Mul); break;
                case CodeType.Div: Generator.Emit(OpCodes.Div); break;
                case CodeType.Mod: Generator.Emit(OpCodes.Rem); break;
                case CodeType.This: Generator.Emit(OpCodes.Ldarg_0); break;
                case CodeType.Echo: Generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(Int32) })); break;
                default: throw new ArgumentException();
            }
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

        public virtual void GenerateLoad(FullPath name)
        {
            dynamic temp = Root.GetBuilder(name);
            BuildLoad(temp);
        }

        protected void BuildLoad(LocalBuilder local)
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

        protected void BuildLoad(FieldBuilder field)
        {
            if (field.IsStatic)
            {
                Generator.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                Generator.Emit(OpCodes.Ldfld, field);
            }
        }

        protected void BuildLoad(ParameterBuilder param)
        {
            int index = param.Position - 1;
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

        public virtual void GenerateStore(FullPath name)
        {
            dynamic temp = Root.GetBuilder(name);
            BuildStore(temp);
        }

        protected void BuildStore(LocalBuilder local)
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

        protected void BuildStore(FieldBuilder field)
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

        protected void BuildStore(ParameterBuilder param)
        {
            int index = param.Position - 1;
            if (index <= 255)
            {
                Generator.Emit(OpCodes.Starg_S, index);
            }
            else
            {
                Generator.Emit(OpCodes.Starg, index);
            }
        }

        public virtual void GenerateCall(FullPath name)
        {
            var temp = Root.GetBuilder(name);
            if (temp is Type)
            {
                var ctor = Root.GetConstructor(name);
                Generator.Emit(OpCodes.Newobj, ctor);
            }
            else
            {
                Generator.Emit(OpCodes.Call, temp);
            }
        }
    }
}
