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
        private TypeBuilder Lexical;

        public RoutineTranslator(FullPath path, Translator parent, MethodBuilder method)
            : base(path, parent)
        {
            Method = method;
            Generator = Method.GetILGenerator();
            Root.RegisterBuilder(path, Method);
        }

        private void PrepareLexical()
        {
            if(Lexical == null)
            {
                Lexical = Parent.CreateLexicalBuilder();
            }
        }

        internal override TypeBuilder CreateLexicalBuilder()
        {
            return Lexical.DefineNestedType("@@lexical", TypeAttributes.SpecialName);
        }

        public override void Save()
        {
            base.Save();
            if (Lexical != null)
            {
                Lexical.CreateType();
            }
        }

        public override RoutineTranslator CreateRoutine(FullPath path)
        {
            PrepareLexical();
            var builder = Lexical.DefineMethod(path.Name, MethodAttributes.Public);
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
    }
}
