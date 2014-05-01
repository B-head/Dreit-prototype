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
    public class ClassTranslator : Translator
    {
        private TypeBuilder Class;
        private MethodBuilder ClassContext;
        private Dictionary<Scope, dynamic> InitDictonary;
        private ILGenerator CtorGenerator;

        public ClassTranslator(Scope path, Translator parent, TypeBuilder builder)
            : base(path, parent)
        {
            Class = builder;
            ClassContext = Class.DefineMethod("@@init", MethodAttributes.SpecialName | MethodAttributes.Static);
            parent.BuildInitCall(ClassContext);
            InitDictonary = new Dictionary<Scope, dynamic>();
            Generator = ClassContext.GetILGenerator();
            Root.RegisterBuilder(path, Class);
            CreateConstructor(path);
        }

        public override void BuildCode()
        {
            base.BuildCode();
            CtorGenerator.Emit(OpCodes.Ret);
            Class.CreateType();
        }

        private ConstructorBuilder CreateConstructor(Scope path)
        {
            var ctor = Class.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, Type.EmptyTypes);
            Root.RegisterBuilder(path, ctor);
            CtorGenerator = ctor.GetILGenerator();
            CtorGenerator.Emit(OpCodes.Ldarg_0);
            CtorGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            return ctor;
        }

        internal override TypeBuilder CreateLexical(string name)
        {
            return Class.DefineNestedType(name + "@@lexical", TypeAttributes.SpecialName);
        }

        public override RoutineTranslator CreateRoutine(Scope path, Scope returnType, Scope[] argumentType)
        {
            var retbld = Root.GetReturnBuilder(returnType);
            var argbld = Root.GetArgumentBuilders(argumentType);
            var builder = Class.DefineMethod(path.Name, MethodAttributes.Public, retbld, argbld);
            return new RoutineTranslator(path, this, builder);
        }

        public override ClassTranslator CreateClass(Scope path)
        {
            var builder = Class.DefineNestedType(path.Name);
            return new ClassTranslator(path, this, builder);
        }

        public override void CreateVariant(Scope path, Scope typeName)
        {
            var type = Root.GetBuilder(typeName);
            var builder = Class.DefineField(path.Name, type, FieldAttributes.Public);
            Root.RegisterBuilder(path, builder);
            var init = Class.DefineField(path.Name + "@@default", type, FieldAttributes.Static | FieldAttributes.SpecialName);
            InitDictonary.Add(path, init);
            CtorGenerator.Emit(OpCodes.Ldarg_0);
            CtorGenerator.Emit(OpCodes.Ldsfld, init);
            CtorGenerator.Emit(OpCodes.Stfld, builder);
        }

        public override void GenerateLoad(Scope name, bool address = false)
        {
            if (name is ThisSymbol)
            {
                GenerateLoad((ThisSymbol)name);
                return;
            }
            dynamic temp;
            if (!InitDictonary.TryGetValue(name, out temp))
            {
                temp = Root.GetBuilder(name);
            }
            BuildLoad(temp, address);
        }

        public override void GenerateStore(Scope name, bool address = false)
        {
            if (name is ThisSymbol)
            {
                GenerateStore((ThisSymbol)name);
                return;
            }
            dynamic temp;
            if (!InitDictonary.TryGetValue(name, out temp))
            {
                temp = Root.GetBuilder(name);
            }
            BuildStore(temp, address);
        }
    }
}
