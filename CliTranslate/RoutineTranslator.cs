using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Symbol;

namespace CliTranslate
{
    public class RoutineTranslator : Translator
    {
        private MethodBase Method;
        private TypeBuilder Lexical;
        private LocalBuilder LexicalInstance;

        public RoutineTranslator(Scope path, Translator parent, MethodBuilder method, bool isDestructor = false)
            : base(path, parent)
        {
            Method = method;
            Root.RegisterBuilder(path, method);
            Generator = method.GetILGenerator();
            if(isDestructor)
            {
                Generator.Emit(OpCodes.Ldarg_0);
                Generator.Emit(OpCodes.Call, typeof(object).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance));
                Generator.Emit(OpCodes.Ret);
            }
        }

        public RoutineTranslator(Scope path, Translator parent, ConstructorBuilder method, MethodBuilder init)
            : base(path, parent)
        {
            Method = method;
            Root.RegisterBuilder(path, method);
            Generator = method.GetILGenerator();
            Generator.Emit(OpCodes.Ldarg_0);
            Generator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            Generator.Emit(OpCodes.Ldarg_0);
            Generator.Emit(OpCodes.Call, init);
            if(path is DefaultSymbol)
            {
                Generator.Emit(OpCodes.Ret);
            }
        }

        public override bool IsThisArg
        {
            get { return Parent is ClassTranslator; }
        }

        public override void BuildCode()
        {
            base.BuildCode();
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
                BuildStore(LexicalInstance, false);
            }
        }

        internal override TypeBuilder CreateLexical(string name)
        {
            return Lexical.DefineNestedType(name + "@@lexical", TypeAttributes.SpecialName | TypeAttributes.NestedPrivate);
        }

        public override RoutineTranslator CreateRoutine(Scope path, Scope returnType, Scope[] argumentType)
        {
            PrepareLexical();
            var retbld = Root.GetReturnBuilder(returnType);
            var argbld = Root.GetArgumentBuilders(argumentType);
            var builder = Lexical.DefineMethod(path.Name, MethodAttributes.Public, retbld, argbld);
            return new RoutineTranslator(path, this, builder);
        }

        public override ClassTranslator CreateClass(Scope path)
        {
            PrepareLexical();
            var builder = Lexical.DefineNestedType(path.Name);
            return new ClassTranslator(path, this, builder);
        }

        public override void CreateVariant(Scope path, Scope type)
        {
            PrepareLexical();
            //var builder = Lexical.DefineField(path.Name, Root.GetBuilder(type), FieldAttributes.Public);
            var builder = Generator.DeclareLocal(Root.GetBuilder(type));
            Root.RegisterBuilder(path, builder);
        }

        public void CreateArguments(Scope[] path)
        {
            int next = Parent is PrimitiveTranslator ? 2 : 1;
            foreach (var v in path)
            {
                if (Method is MethodBuilder)
                {
                    var m = (MethodBuilder)Method;
                    var builder = m.DefineParameter(next++, ParameterAttributes.None, v.Name);
                    Root.RegisterBuilder(v, builder);
                }
                else if (Method is ConstructorBuilder)
                {
                    var c = (ConstructorBuilder)Method;
                    var builder = c.DefineParameter(next++, ParameterAttributes.None, v.Name);
                    Root.RegisterBuilder(v, builder);
                }
            }
        }

        public override void GenerateLoad(Scope name, bool address = false)
        {
            if (name is ThisSymbol)
            {
                GenerateLoad((ThisSymbol)name, address);
                return;
            }
            dynamic temp = Root.GetBuilder(name);
            //FieldBuilder field = temp as FieldBuilder;
            //if(field != null && field.DeclaringType == Lexical)
            //{
            //    BuildLoad(LexicalInstance, false);
            //}
            BuildLoad(temp, address);
        }

        public override void GenerateStore(Scope name, bool address = false)
        {
            if (name is ThisSymbol)
            {
                GenerateStore((ThisSymbol)name, address);
                return;
            }
            dynamic temp = Root.GetBuilder(name);
            //FieldBuilder field = temp as FieldBuilder;
            //if (field != null && field.DeclaringType == Lexical)
            //{
            //    BuildLoad(LexicalInstance, false);
            //}
            BuildStore(temp, address);
        }
    }
}
