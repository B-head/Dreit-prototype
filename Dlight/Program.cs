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
            assembly.CreateScope();
            ErrorManager manager = new ErrorManager();
            assembly.CheckSemantic(manager);
            Console.WriteLine(manager);
            Console.WriteLine(assembly);
            if (manager.ErrorCount == 0)
            {
                AssemblyTranslator trans = new AssemblyTranslator(fileName.Replace(".txt", ""));
                RegisterEmbedType(trans);
                assembly.Translate(trans);
                trans.Save();
            }
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
            trans.RegisterEmbedType("Binary64", typeof(DlightObject.Binary64));
        }
    }
}
