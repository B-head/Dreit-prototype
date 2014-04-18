using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;

namespace CliTranslate
{
    public class RoutineTranslator : Translator
    {
        private MethodBuilder Method;
        private TypeBuilder Lexical;
        private LocalBuilder LexicalInstance;

        public RoutineTranslator(FullPath path, Translator parent, MethodBuilder method)
            : base(path, parent)
        {
            Method = method;
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

        private void PrepareLexical()
        {
            if(Lexical == null)
            {
                Lexical = Parent.CreateLexical(Method.Name);
                LexicalInstance = Generator.DeclareLocal(Lexical);
                Generator.Emit(OpCodes.Newobj, Lexical.DefineDefaultConstructor(MethodAttributes.PrivateScope));
                BuildStore(LexicalInstance);
            }
        }

        internal override TypeBuilder CreateLexical(string name)
        {
            return Lexical.DefineNestedType(name + "@@lexical", TypeAttributes.SpecialName);
        }

        public override RoutineTranslator CreateRoutine(FullPath path, FullPath returnType, FullPath[] argumentType)
        {
            PrepareLexical();
            var retbld = Root.GetReturnBuilder(returnType);
            var argbld = Root.GetArgumentBuilders(argumentType);
            var builder = Lexical.DefineMethod(path.Name, MethodAttributes.Public, retbld, argbld);
            return new RoutineTranslator(path, this, builder);
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

        public void CreateArguments(FullPath[] path)
        {
            int next = Parent is PrimitiveTranslator ? 2 : 1;
            foreach (var v in path)
            {
                var builder = Method.DefineParameter(next++, ParameterAttributes.None, v.Name);
                Root.RegisterBuilder(v, builder);
            }
        }

        public override void GenerateLoad(FullPath name)
        {
            dynamic temp = Root.GetBuilder(name);
            FieldBuilder field = temp as FieldBuilder;
            if(field != null && field.DeclaringType == Lexical)
            {
                BuildLoad(LexicalInstance);
            }
            BuildLoad(temp);
        }

        public override void GenerateStore(FullPath name)
        {
            dynamic temp = Root.GetBuilder(name);
            FieldBuilder field = temp as FieldBuilder;
            if (field != null && field.DeclaringType == Lexical)
            {
                BuildLoad(LexicalInstance);
            }
            BuildStore(temp);
        }
    }
}
