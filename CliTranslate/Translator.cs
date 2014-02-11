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

        internal virtual TypeBuilder CreateLexicalBuilder()
        {
            throw new NotSupportedException();
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

        public virtual ModuleTranslator CreateModule(FullPath path)
        {
            throw new NotSupportedException();
        }

        public virtual RoutineTranslator CreateRoutine(FullPath path)
        {
            throw new NotSupportedException();
        }

        public void ImportRoutine(FullPath path)
        {
            var type = Root.GetImportType(path.GetNameSpace());
            if (type == null)
            {
                return;
                throw new Exception();
            }
            var member = type.GetMember(path.Name);
            if(member == null || member.Length == 0)
            {
                return;
            }
            Root.RegisterBuilder(path, member[0]);
            //var ctor = type.GetConstructors()[0];
            //Root.RegisterBuilder(path, ctor); //手抜きｗ
        }

        public virtual RoutineTranslator CreateOperation(CodeType operation)
        {
            throw new NotSupportedException();
        }

        public virtual ClassTranslator CreateClass(FullPath path)
        {
            throw new NotSupportedException();
        }

        public void ImportClass(FullPath path)
        {
            var type = Root.GetImportType(path.ToString());
            if(type == null)
            {
                return;
                throw new Exception();
            }
            Root.RegisterBuilder(path, type);
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

        public void GenelateControl(CodeType type)
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
                default: throw new ArgumentException();
            }
        }

        public void GenelatePrimitive(int value)
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

        public void GenelatePrimitive(long value)
        {
            Generator.Emit(OpCodes.Ldc_I8, value);
        }

        public void GenelatePrimitive(float value)
        {
            Generator.Emit(OpCodes.Ldc_R4, value);
        }

        public void GenelatePrimitive(double value)
        {
            Generator.Emit(OpCodes.Ldc_R8, value);
        }

        public void GenelatePrimitive(string value)
        {
            Generator.Emit(OpCodes.Ldstr, value);
        }

        public void GenelateLoad(FullPath name)
        {
            BuildLoad(Root.GetBuilder(name));
        }

        private void BuildLoad(LocalBuilder local)
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

        private void BuildLoad(FieldBuilder field)
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

        public virtual void GenelateStore(FullPath name)
        {
            BuildStore(Root.GetBuilder(name));
        }

        private void BuildStore(LocalBuilder local)
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

        private void BuildStore(FieldBuilder field)
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

        public virtual void GenelateCall(FullPath name)
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
    }
}
