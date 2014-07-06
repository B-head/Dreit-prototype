using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Pragma;
using AbstractSyntax.Daclate;

namespace CliTranslate
{
    public class ModuleTranslator : Translator
    {
        private ModuleBuilder Module;
        private TypeBuilder GlobalField;
        private MethodBuilder EntryContext;

        public ModuleTranslator(DeclateModule path, Translator parent, ModuleBuilder module)
            : base(path, parent)
        {
            Module = module;
            GlobalField = Module.DefineType(path.Name + ".@@global", TypeAttributes.SpecialName);
            EntryContext = GlobalField.DefineMethod("@@entry", MethodAttributes.SpecialName | MethodAttributes.Static, null, null);
            Generator = EntryContext.GetILGenerator();
            Root.SetEntryPoint(EntryContext);
        }

        public override void BuildCode()
        {
            base.BuildCode();
            GlobalField.CreateType();
        }

        internal override TypeBuilder CreateLexical(string name)
        {
            return Module.DefineType(name + "@@lexical", TypeAttributes.SpecialName);
        }

        public override RoutineTranslator CreateRoutine(DeclateRoutine path)
        {
            var retbld = Root.GetTypeBuilder(path.CallReturnType);
            var argbld = Root.GetTypeBuilders(path.ArgumentTypes);
            var attr = MakeMethodAttributes(path.Attribute, path.IsVirtual) | MethodAttributes.Static;
            var builder = GlobalField.DefineMethod(path.Name, attr, retbld, argbld);
            return new RoutineTranslator(path, this, builder);
        }

        public override ClassTranslator CreateClass(DeclateClass path)
        {
            var cls = Root.GetTypeBuilder(path.InheritClass);
            var trait = Root.GetTypeBuilders(path.InheritTraits);
            var attr = MakeTypeAttributes(path.Attribute, path.IsTrait);
            var builder = Module.DefineType(path.FullName, attr, cls, trait);
            return new ClassTranslator(path, this, builder);
        }

        public override PrimitiveTranslator CreatePrimitive(DeclateClass path)
        {
            var attr = MakeTypeAttributes(path.Attribute, path.IsTrait);
            var builder = Module.DefineType(path.FullName, attr);
            return new PrimitiveTranslator(path, this, builder);
        }

        public override void CreateVariant(DeclateVariant path)
        {
            var attr = MakeFieldAttributes(path.Attribute) | FieldAttributes.Static;
            var builder = GlobalField.DefineField(path.Name, Root.GetBuilder(path.ReturnType), attr);
            Root.RegisterBuilder(path, builder);
        }
    }
}
