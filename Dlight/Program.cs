using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using LexicalAnalysis;
using SyntacticAnalysis;
using CliTranslate;
using AbstractSyntax;
using CliImport;
using Common;

namespace Dlight
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = args[0];
            Root root = new Root();
            ImportManager.ImportAssembly(root, Assembly.Load("DlightObject"));
            ImportManager.ImportAssembly(root, Assembly.Load("mscorlib"));
            root.Append(CompileFile(fileName));
            root.SemanticAnalysis();
            Console.WriteLine(root.CompileResult());
            //Console.WriteLine(root);
            if (root.ErrorCount == 0)
            {
                RootTranslator trans = new RootTranslator();
                //root.TranslateTo(trans);
                //trans.Save(fileName.Replace(".txt", ""));
            }
        }

        static Element CompileFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            Lexer lexer = new Lexer();
            List<Token> token = lexer.Lex(text, fileName);
            Parser parser = new Parser();
            return parser.Parse(token, fileName.Replace(".txt", ""));
        }
    }
}
