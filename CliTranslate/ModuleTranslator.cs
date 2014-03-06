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
    public class ModuleTranslator : Translator
    {
        private ModuleBuilder Module;
        private TypeBuilder GlobalField;
        private MethodBuilder EntryContext;

        public ModuleTranslator(FullPath path, Translator parent, ModuleBuilder module)
            : base(path, parent)
        {
            Module = module;
            GlobalField = Module.DefineType("@@global", TypeAttributes.SpecialName);
            EntryContext = Module.DefineGlobalMethod("@@entry", MethodAttributes.SpecialName | MethodAttributes.Static, null, null);
            Generator = EntryContext.GetILGenerator();
            Root.SetEntryPoint(EntryContext);
        }

        public override void Save()
        {
            base.Save();
            GlobalField.CreateType();
        }

        internal override TypeBuilder CreateLexical()
        {
            return Module.DefineType("@@lexical", TypeAttributes.SpecialName);
        }

        public override RoutineTranslator CreateRoutine(FullPath path, FullPath returnType)
        {
            var builder = Module.DefineGlobalMethod(path.Name, MethodAttributes.Static, null, null);
            return new RoutineTranslator(path, this, builder, returnType); //モジュールに直接レキシカルオブジェクトを作りたい。
        }

        public override ClassTranslator CreateClass(FullPath path)
        {
            var builder = Module.DefineType(path.ToString());
            return new ClassTranslator(path, this, builder);
        }

        public override void CreateVariant(FullPath path, FullPath type)
        {
            var builder = GlobalField.DefineField(path.Name, Root.GetBuilder(type), FieldAttributes.Static);
            Root.RegisterBuilder(path, builder);
        }
    }
}
