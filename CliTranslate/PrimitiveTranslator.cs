using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using AbstractSyntax;
using AbstractSyntax.Pragma;
using AbstractSyntax.Declaration;

namespace CliTranslate
{
    public class PrimitiveTranslator : Translator
    {
        private TypeBuilder Class;
        internal Type Prim { get; private set; }
        private MethodBuilder ClassContext;

        public PrimitiveTranslator(ClassDeclaration path, Translator parent, TypeBuilder builder)
            : base(path, parent)
        {
            Class = builder;
            Prim = GetPrimitiveType(path.PrimitiveType);
            ClassContext = Class.DefineMethod("@@init", MethodAttributes.SpecialName | MethodAttributes.Static);
            parent.GenerateCall(ClassContext);
            Generator = ClassContext.GetILGenerator();
            Root.RegisterBuilder(path, Prim);
            var ctor = Prim.GetConstructor(Type.EmptyTypes);
            if(ctor != null)
            {
                Root.RegisterBuilder(path.DefaultInitializer, ctor);
            }
        }

        internal static Type GetPrimitiveType(CastPragmaType type)
        {
            switch(type)
            {
                case CastPragmaType.Object: return typeof(Object);
                case CastPragmaType.String: return typeof(String);
                case CastPragmaType.Boolean: return typeof(Boolean);
                case CastPragmaType.Integer8: return typeof(SByte);
                case CastPragmaType.Integer16: return typeof(Int16);
                case CastPragmaType.Integer32: return typeof(Int32);
                case CastPragmaType.Integer64: return typeof(Int64);
                case CastPragmaType.Natural8: return typeof(Byte);
                case CastPragmaType.Natural16: return typeof(UInt16);
                case CastPragmaType.Natural32: return typeof(UInt32);
                case CastPragmaType.Natural64: return typeof(UInt64);
                case CastPragmaType.Binary32: return typeof(Single);
                case CastPragmaType.Binary64: return typeof(Double);
                default: throw new ArgumentException();
            }
        }

        public override void BuildCode()
        {
            base.BuildCode();
            Class.CreateType();
        }

        internal override TypeBuilder CreateLexical(string name)
        {
            return Class.DefineNestedType(name + "@@lexical", TypeAttributes.SpecialName | TypeAttributes.NestedPrivate);
        }

        public override RoutineTranslator CreateRoutine(RoutineDeclaration path)
        {
            var attr = MakeMethodAttributes(path.Attribute, path.IsVirtual) | MethodAttributes.Static;
            var builder = Class.DefineMethod(path.Name, attr);
            return new RoutineTranslator(path, this, builder);
        }
    }
}
