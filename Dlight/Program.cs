using AbstractSyntax;
using CliTranslate;
using SyntacticAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            List<Token> tokenList, errorToken;
            TextPosition position;
            Lexer.Lex(text, fileName, out tokenList, out errorToken, out position);
            string name = fileName.Replace(".dl", "").Split('/').Last();
            Parser parser = new Parser();
            return parser.Parse(text, name, tokenList, errorToken, position);
        }
    }
}
