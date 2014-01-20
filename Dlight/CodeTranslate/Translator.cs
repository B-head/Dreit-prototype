using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CodeTranslate
{
    class Translator
    {
        public void Trans(Root root, string save)
        {
            AssemblyName name = new AssemblyName(root.Position.File);
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assembly.DefineDynamicModule(root.Position.File, save);
            MethodAttributes attr = MethodAttributes.Static | MethodAttributes.Public;
            MethodBuilder method = module.DefineGlobalMethod("@@entrypoint", attr, typeof(void), Type.EmptyTypes);
            ILGenerator generator = method.GetILGenerator();
            ChildrenGenerate(generator, root);
            generator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }), new Type[] { typeof(int) });
            generator.Emit(OpCodes.Ret);
            module.CreateGlobalFunctions();
            assembly.SetEntryPoint(method);
            assembly.Save(save);
        }

        private void ChildrenGenerate(ILGenerator generator, AbstractSyntax syntax)
        {
            foreach(AbstractSyntax v in syntax)
            {
                Generate(generator, (dynamic)v);
            }
        }

        private void Generate(ILGenerator generator, Binomial syntax)
        {
            ChildrenGenerate(generator, syntax);
            switch(syntax.Operation)
            {
                case SyntaxType.Plus: generator.Emit(OpCodes.Add); break;
                case SyntaxType.Minus: generator.Emit(OpCodes.Sub); break;
                case SyntaxType.Multiply: generator.Emit(OpCodes.Mul); break;
                case SyntaxType.Divide: generator.Emit(OpCodes.Div); break;
                case SyntaxType.Modulo: generator.Emit(OpCodes.Rem); break;
                case SyntaxType.Or: generator.Emit(OpCodes.Or); break;
                case SyntaxType.And: generator.Emit(OpCodes.And); break;
                case SyntaxType.Xor: generator.Emit(OpCodes.Xor); break;
                case SyntaxType.LeftShift: generator.Emit(OpCodes.Shl); break;
                case SyntaxType.RightShift: generator.Emit(OpCodes.Shr); break;
                default: throw new ArgumentException();
            }
        }

        private void Generate(ILGenerator generator, NumberLiteral syntax)
        {
            dynamic number;
            syntax.TryParse(out number);
            generator.Emit(OpCodes.Ldc_I4, (int)number);
        }
    }
}
