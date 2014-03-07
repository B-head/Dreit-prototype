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
            ImportManager import = new ImportManager(root);
            //import.ImportAssembly(Assembly.Load("mscorlib"));
            //import.ImportAssembly(Assembly.Load("DlightObject"));
            root.Append(CompileFile("lib/primitive.dl"));
            root.Append(CompileFile(fileName));
            root.SemanticAnalysis();
            Console.WriteLine(root.ToString(true));
            Console.WriteLine(root.CompileResult());
            if (root.ErrorCount == 0)
            {
                RootTranslator trans = new RootTranslator(fileName.Replace(".dl", ""));
                import.TranslateImport(trans);
                root.TranslateTo(trans);
                trans.Save();
            }
        }

        static Element CompileFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            Lexer lexer = new Lexer();
            List<Token> token = lexer.Lex(text, fileName);
            Parser parser = new Parser();
            return parser.Parse(token, fileName.Replace(".dl", "").Split('/').Last());
        }
    }
}
