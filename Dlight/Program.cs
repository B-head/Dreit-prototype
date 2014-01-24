using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Emit;
using Dlight.LexicalAnalysis;
using Dlight.SyntacticAnalysis;
using Dlight.CilTranslate;

namespace Dlight
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = args[0];
            List<ModuleElement> module = new List<ModuleElement>();
            module.Add(CompileFile(fileName));
            AssemblyElement assembly = new AssemblyElement(fileName.Replace(".txt", ""), module);
            Console.WriteLine(assembly);
            assembly.CreateScope();
            assembly.CheckSemantic(Console.Error);
            AssemblyTranslator trans = new AssemblyTranslator(fileName.Replace(".txt", ""));
            RegisterEmbedType(trans);
            assembly.Translate(trans);
        }

        static ModuleElement CompileFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            Lexer lexer = new Lexer();
            List<Token> token = lexer.Lex(text, fileName);
            Parser parser = new Parser();
            return parser.Parse(token, fileName.Replace(".txt", ""));
        }

        static void RegisterEmbedType(AssemblyTranslator trans)
        {
            trans.RegisterEmbedType("Integer32", typeof(DlightObject.Integer32));
        }
    }
}
