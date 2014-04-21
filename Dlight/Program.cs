using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using SyntacticAnalysis;
using CliTranslate;
using AbstractSyntax;

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
            root.Append(CompileFile("lib/primitive.dl"));
            root.Append(CompileFile(fileName));
            root.SemanticAnalysis();
            //Console.WriteLine(root.ToString(true));
            Console.WriteLine(root.CompileResult());
            if (root.ErrorCount == 0)
            {
                TranslateManager trans = new TranslateManager(fileName.Replace(".dl", ""));
                trans.TranslateTo(root, import);
            }
        }

        static Element CompileFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            Lexer lexer = new Lexer();
            lexer.Lex(text, fileName);
            Parser parser = new Parser();
            return parser.Parse(lexer, fileName.Replace(".dl", "").Split('/').Last());
        }
    }
}
