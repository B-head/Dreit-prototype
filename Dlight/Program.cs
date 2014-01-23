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
            List<Module> module = new List<Module>();
            module.Add(CompileFile(fileName));
            Assembly assembly = new Assembly(fileName.Replace(".txt", ""), module);
            Console.WriteLine(assembly);
            assembly.CreateScope();
            assembly.CheckSemantic(Console.Error);
            AssemblyTranslator trans = new AssemblyTranslator(fileName.Replace(".txt", ""));
            assembly.Translate(trans);
        }

        static Module CompileFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            Lexer lexer = new Lexer();
            List<Token> token = lexer.Lex(text, fileName);
            Parser parser = new Parser();
            return parser.Parse(token, fileName.Replace(".txt", ""));
        }
    }
}
