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
    public class ClassTranslator : Translator
    {
        private TypeBuilder Class;
        private MethodBuilder ClassContext;
        private Dictionary<FullPath, dynamic> InitDictonary;
        private ILGenerator CtorGenerator;

        public ClassTranslator(FullPath path, Translator parent, TypeBuilder builder)
            : base(path, parent)
        {
            Class = builder;
            ClassContext = Class.DefineMethod("@@init", MethodAttributes.SpecialName | MethodAttributes.Static);
            parent.BuildInitCall(ClassContext);
            InitDictonary = new Dictionary<FullPath, dynamic>();
            Generator = ClassContext.GetILGenerator();
            Root.RegisterBuilder(path, Class);
            CreateConstructor(path);
        }

        public override void Save()
        {
            base.Save();
            CtorGenerator.Emit(OpCodes.Ret);
            Class.CreateType();
        }

        private ConstructorBuilder CreateConstructor(FullPath path)
        {
            var ctor = Class.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, Type.EmptyTypes);
            Root.RegisterBuilder(path, ctor);
            CtorGenerator = ctor.GetILGenerator();
            CtorGenerator.Emit(OpCodes.Ldarg_0);
            CtorGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            return ctor;
        }

        internal override TypeBuilder CreateLexical()
        {
            return Class.DefineNestedType("@@lexical", TypeAttributes.SpecialName);
        }

        public override RoutineTranslator CreateRoutine(FullPath path, FullPath returnType)
        {
            var builder = Class.DefineMethod(path.Name, MethodAttributes.Public);
            return new RoutineTranslator(path, this, builder, returnType);
        }

        public override ClassTranslator CreateClass(FullPath path)
        {
            var builder = Class.DefineNestedType(path.Name);
            return new ClassTranslator(path, this, builder);
        }

        public override void CreateVariant(FullPath path, FullPath typeName)
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

        public override void GenerateLoad(FullPath name)
        {
            dynamic temp;
            if (!InitDictonary.TryGetValue(name, out temp))
            {
                temp = Root.GetBuilder(name);
            }
            BuildLoad(temp);
        }

        public override void GenerateStore(FullPath name)
        {
            dynamic temp;
            if (!InitDictonary.TryGetValue(name, out temp))
            {
                temp = Root.GetBuilder(name);
            }
            BuildStore(temp);
        }
    }
}
