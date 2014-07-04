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
        public IScope Path { get; private set; }
        internal ILGenerator Generator;

        protected Translator(IScope path, Translator parent)
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

        public virtual void BuildCode()
        {
            foreach (var v in _Child)
            {
                v.BuildCode();
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + ": " + Path.FullName;
        }

        public virtual bool IsThisArg
        {
            get { return false; }
        }

        protected TypeAttributes MakeTypeAttributes(IReadOnlyList<IScope> attr, bool isTrait = false)
        {
            TypeAttributes ret = isTrait ? TypeAttributes.Interface | TypeAttributes.Abstract : TypeAttributes.Class;
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                switch (a.Attr)
                {
                    case AttributeType.Public: ret |= TypeAttributes.NotPublic; break;
                    case AttributeType.Protected: ret |= TypeAttributes.NotPublic; break;
                    case AttributeType.Private: ret |= TypeAttributes.NotPublic; break;
                }
            }
            return ret;
        }

        protected TypeAttributes MakeNestedTypeAttributes(IReadOnlyList<IScope> attr, bool isTrait = false)
        {
            TypeAttributes ret = isTrait ? TypeAttributes.Interface | TypeAttributes.Abstract : TypeAttributes.Class;
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                switch (a.Attr)
                {
                    case AttributeType.Public: ret |= TypeAttributes.Public | TypeAttributes.NestedAssembly; break;
                    case AttributeType.Protected: ret |= TypeAttributes.NotPublic | TypeAttributes.NestedFamily; break;
                    case AttributeType.Private: ret |= TypeAttributes.NotPublic | TypeAttributes.NestedPrivate; break;
                }
            }
            return ret;
        }

        protected MethodAttributes MakeMethodAttributes(IReadOnlyList<IScope> attr, bool isVirtual = false)
        {
            MethodAttributes ret = isVirtual ? MethodAttributes.Virtual | MethodAttributes.ReuseSlot : MethodAttributes.ReuseSlot;
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                switch (a.Attr)
                {
                    case AttributeType.Static: ret |= MethodAttributes.Static; break;
                    case AttributeType.Public: ret |= MethodAttributes.Assembly; break;
                    case AttributeType.Protected: ret |= MethodAttributes.Family; break;
                    case AttributeType.Private: ret |= MethodAttributes.Private; break;
                }
            }
            return ret;
        }

        protected FieldAttributes MakeFieldAttributes(IReadOnlyList<IScope> attr)
        {
            FieldAttributes ret = 0;
            foreach (var v in attr)
            {
                var a = v as AttributeSymbol;
                if (a == null)
                {
                    continue;
                }
                switch (a.Attr)
                {
                    case AttributeType.Static: ret |= FieldAttributes.Static; break;
                    case AttributeType.Public: ret |= FieldAttributes.Assembly; break;
                    case AttributeType.Protected: ret |= FieldAttributes.Family; break;
                    case AttributeType.Private: ret |= FieldAttributes.Private; break;
                }
            }
            return ret;
        }

        internal virtual TypeBuilder CreateLexical(string name)
        {
            return Parent.CreateLexical(name);
        }

        public virtual ModuleTranslator CreateModule(DeclateModule path)
        {
            return Parent.CreateModule(path);
        }

        public virtual RoutineTranslator CreateRoutine(DeclateRoutine path)
        {
            return Parent.CreateRoutine(path);
        }

        public virtual ClassTranslator CreateClass(DeclateClass path)
        {
            return Parent.CreateClass(path);
        }

        public virtual PrimitiveTranslator CreatePrimitive(DeclateClass path)
        {
            return Parent.CreatePrimitive(path);
        }

        public virtual void CreateEnum(DeclateEnum path)
        {
            Parent.CreateEnum(path);
        }

        public virtual void CreateVariant(DeclateVariant path)
        {
            Parent.CreateVariant(path);
        }

        public BranchTranslator CreateBranch(IScope path, bool definedElse = false)
        {
            return new BranchTranslator(path, this, definedElse);
        }

        public LoopTranslator CreateLoop(IScope path)
        {
            return new LoopTranslator(path, this);
        }

        public Label CreateLabel(IScope path = null)
        {
            var builder = Generator.DefineLabel();
            if (path != null)
            {
                Root.RegisterBuilder(path, builder);
            }
            return builder;
        }

        public Label GetLabel(IScope path)
        {
            if (Root.ContainsBuilder(path))
            {
                return (Label)Root.GetBuilder(path);
            }
            else
            {
                return CreateLabel(path);
            }
        }

        public virtual Label GetContinueLabel()
        {
            return Parent.GetContinueLabel();
        }

        public virtual Label GetBreakLabel()
        {
            return Parent.GetBreakLabel();
        }

        public void MarkLabel(Label label)
        {
            Generator.MarkLabel(label);
        }

        public void BeginScope()
        {
            Generator.BeginScope();
        }

        public void EndScope()
        {
            Generator.EndScope();
        }

        public void GenerateJump(OpCode type, Label label)
        {
            Generator.Emit(type, label);
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

        public void GeneratePrimitive(bool value)
        {
            if(value)
            {
                Generator.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                Generator.Emit(OpCodes.Ldc_I4_0);
            }
        }

        public virtual void GenerateLoad(IScope name, bool address = false)
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
            var c = name.ReturnType as DeclateClass;
            var pe = c.PrimitiveType;
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

        public virtual void GenerateStore(IScope name, bool address = false)
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
            var c = name.ReturnType as DeclateClass;
            var pe = c.PrimitiveType;
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

        public virtual void GenerateCall(IScope name)
        {
            var r = name as RoutineSymbol;
            var temp = Root.GetBuilder(name);
            if (temp is ConstructorInfo)
            {
                Generator.Emit(OpCodes.Newobj, temp);
            }
            else if (r != null && r.IsVirtual)
            {
                Generator.Emit(OpCodes.Callvirt, temp);
            }
            else
            {
                Generator.Emit(OpCodes.Call, temp);
            }
        }

        internal void GenerateCall(MethodBuilder method)
        {
            Generator.Emit(OpCodes.Call, method);
        }

        public void GenerateEcho(IScope type)
        {
            var temp = Root.GetBuilder(type) as Type;
            var types = new Type[] { temp };
            Generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", types));
        }

        public void GenerateToString(IScope type)
        {
            var t = Root.GetBuilder(type) as Type;
            if(t.IsValueType)
            {
                Generator.Emit(OpCodes.Box, t);
            }
            Generator.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString", Type.EmptyTypes));
        }

        public void GenerateStringConcat()
        {
            Generator.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }));
        }
    }
}
