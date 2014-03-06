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
    public class RoutineTranslator : Translator
    {
        private MethodBuilder Method;
        private List<FullPath> ParamName;
        private List<Type> ParamType;
        private TypeBuilder Lexical;
        private LocalBuilder LexicalInstance;

        public RoutineTranslator(FullPath path, Translator parent, MethodBuilder method, FullPath returnType)
            : base(path, parent)
        {
            Method = method;
            ParamName = new List<FullPath>();
            ParamType = new List<Type>();
            if (returnType != null)
            {
                Method.SetReturnType(Root.GetBuilder(returnType));
            }
            Generator = Method.GetILGenerator();
            Root.RegisterBuilder(path, Method);
        }

        public override void Save()
        {
            base.Save();
            if (Lexical != null)
            {
                Lexical.CreateType();
            }
        }

        public void SaveArgument()
        {
            Method.SetParameters(ParamType.ToArray());
            int next = 1;
            foreach (var v in ParamName)
            {
                var builder = Method.DefineParameter(next++, ParameterAttributes.None, v.Name);
                Root.RegisterBuilder(v, builder);
            }
        }

        private void PrepareLexical()
        {
            if(Lexical == null)
            {
                Lexical = Parent.CreateLexical();
                LexicalInstance = Generator.DeclareLocal(Lexical);
                Generator.Emit(OpCodes.Newobj, Lexical.DefineDefaultConstructor(MethodAttributes.PrivateScope));
                BuildStore(LexicalInstance);
            }
        }

        internal override TypeBuilder CreateLexical()
        {
            return Lexical.DefineNestedType("@@lexical", TypeAttributes.SpecialName);
        }

        public override RoutineTranslator CreateRoutine(FullPath path, FullPath returnType)
        {
            PrepareLexical();
            var builder = Lexical.DefineMethod(path.Name, MethodAttributes.Public);
            return new RoutineTranslator(path, this, builder, returnType);
        }

        public override ClassTranslator CreateClass(FullPath path)
        {
            PrepareLexical();
            var builder = Lexical.DefineNestedType(path.Name);
            return new ClassTranslator(path, this, builder);
        }

        public override void CreateVariant(FullPath path, FullPath type)
        {
            PrepareLexical();
            var builder = Lexical.DefineField(path.Name, Root.GetBuilder(type), FieldAttributes.Public);
            Root.RegisterBuilder(path, builder);
        }

        public void CreateArgument(FullPath path, FullPath type)
        {
            ParamName.Add(path);
            ParamType.Add(Root.GetBuilder(type));
        }

        public override void GenerateLoad(FullPath name)
        {
            dynamic temp = Root.GetBuilder(name);
            FieldBuilder field = temp as FieldBuilder;
            if(field != null && field.DeclaringType == Lexical)
            {
                BuildLoad(LexicalInstance);
            }
            else if (field != null && field.DeclaringType == Method.DeclaringType)
            {
                Generator.Emit(OpCodes.Ldarg_0);
            }
            BuildLoad(temp);
        }

        public override void GenerateStore(FullPath name)
        {
            dynamic temp = Root.GetBuilder(name);
            FieldBuilder field = temp as FieldBuilder;
            if (field != null && field.DeclaringType == Lexical)
            {
                LocalBuilder local = Generator.DeclareLocal(field.FieldType);
                BuildStore(local);
                BuildLoad(LexicalInstance);
                BuildLoad(local);
            }
            else if (field != null && field.DeclaringType == Method.DeclaringType)
            {
                LocalBuilder local = Generator.DeclareLocal(field.FieldType);
                BuildStore(local);
                Generator.Emit(OpCodes.Ldarg_0);
                BuildLoad(local);
            }
            BuildStore(temp);
        }
    }
}
