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

        public ClassTranslator(FullPath path, Translator parent, TypeBuilder builder)
            : base(path, parent)
        {
            Class = builder;
            ClassContext = Class.DefineMethod("@@class", MethodAttributes.SpecialName | MethodAttributes.Static);
            Generator = ClassContext.GetILGenerator();
            Root.RegisterBuilder(path, Class);
        }

        internal override TypeBuilder CreateLexicalBuilder()
        {
            return Class.DefineNestedType("@@lexical", TypeAttributes.SpecialName);
        }

        public override void Save()
        {
            base.Save();
            Class.CreateType();
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

        public override void CreateVariant(FullPath path, FullPath type)
        {
            var builder = Class.DefineField(path.Name, Root.GetBuilder(type), FieldAttributes.Public);
            Root.RegisterBuilder(path, builder);
        }
    }
}
