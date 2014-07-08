using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Symbol;
using AbstractSyntax.Daclate;

namespace CliTranslate
{
    public class RoutineTranslator : Translator
    {
        private MethodBase Method;
        private TypeBuilder Lexical;
        private LocalBuilder LexicalInstance;

        public RoutineTranslator(RoutineSymbol path, Translator parent, MethodBuilder method)
            : base(path, parent)
        {
            Method = method;
            Root.RegisterBuilder(path, method);
            Generator = method.GetILGenerator();
        }

        public RoutineTranslator(RoutineSymbol path, Translator parent, ConstructorBuilder ctor)
            : base(path, parent)
        {
            Method = ctor;
            Root.RegisterBuilder(path, ctor);
            Generator = ctor.GetILGenerator();
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
            //if(Lexical == null)
            //{
            //    Lexical = Parent.CreateLexical(Method.Name);
            //    LexicalInstance = Generator.DeclareLocal(Lexical);
            //    Generator.Emit(OpCodes.Newobj, Lexical.DefineDefaultConstructor(MethodAttributes.PrivateScope));
            //    BuildStore(LexicalInstance, false);
            //}
        }

        internal override TypeBuilder CreateLexical(string name)
        {
            return Lexical.DefineNestedType(name + "@@lexical", TypeAttributes.SpecialName | TypeAttributes.NestedPrivate);
        }

        public override RoutineTranslator CreateRoutine(DeclateRoutine path)
        {
            PrepareLexical();
            var builder = Lexical.DefineMethod(path.Name, MethodAttributes.PrivateScope | MethodAttributes.Final);
            return new RoutineTranslator(path, this, builder);
        }

        public override ClassTranslator CreateClass(DeclateClass path)
        {
            PrepareLexical();
            var builder = Lexical.DefineNestedType(path.Name, TypeAttributes.NestedPrivate);
            return new ClassTranslator(path, this, builder);
        }

        public override void CreateVariant(DeclateVariant path)
        {
            PrepareLexical();
            //var builder = Lexical.DefineField(path.Name, Root.GetBuilder(type), FieldAttributes.PrivateScope);
            var builder = Generator.DeclareLocal(Root.GetBuilder(path.ReturnType));
            Root.RegisterBuilder(path, builder);
        }

        public void CreateGenerics(IEnumerable<GenericSymbol> pars)
        {
            if(pars.Count() == 0)
            {
                return;
            }
            var m = (MethodBuilder)Method;
            List<string> name = new List<string>();
            foreach(var v in pars)
            {
                name.Add(v.Name);
            }
            var gp = m.DefineGenericParameters(name.ToArray());
            var i = 0;
            foreach(var v in pars)
            {
                Root.RegisterBuilder(v, gp[i++]);
            }
        }

        public void CreateReturn(Scope ret)
        {
            var m = (MethodBuilder)Method;
            var t = Root.GetTypeBuilder(ret);
            m.SetReturnType(t);
        }

        public void CreateArguments(IEnumerable<DeclateArgument> args, IEnumerable<Scope> argst = null)
        {
            var next = Parent is PrimitiveTranslator ? 2 : 1;
            if (Method is MethodBuilder)
            {
                var m = (MethodBuilder)Method;
                var pt = Parent as PrimitiveTranslator;
                var t = pt == null ? Root.GetTypeBuilders(argst) : Root.GetTypeBuilders(pt.Prim, argst);
                m.SetParameters(t);
                foreach (var v in args)
                {
                    var builder = m.DefineParameter(next++, ParameterAttributes.None, v.Name);
                    Root.RegisterBuilder(v, builder);
                }
            }
            else if (Method is ConstructorBuilder)
            {
                var c = (ConstructorBuilder)Method;
                foreach (var v in args)
                {
                    var builder = c.DefineParameter(next++, ParameterAttributes.None, v.Name);
                    Root.RegisterBuilder(v, builder);
                }
            }
        }

        public void GenelateConstructorInit(MethodBuilder init, ConstructorInfo ctor)
        {
            Generator.Emit(OpCodes.Ldarg_0);
            Generator.Emit(OpCodes.Call, ctor);
            Generator.Emit(OpCodes.Ldarg_0);
            Generator.Emit(OpCodes.Call, init);
            if (Path is DefaultSymbol)
            {
                Generator.Emit(OpCodes.Ret);
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
