using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Emit;
using Dlight.LexicalAnalysis;
using Dlight.SyntacticAnalysis;
using Dlight.SemanticAnalysis;
using Dlight.CodeTranslate;

namespace Dlight
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = args[0];
            string save = file.Replace(".txt", ".exe");
            string text = File.ReadAllText(file);
            Lexer lexer = new Lexer();
            List<Token> token = lexer.Lex(text, file);
            Parser parser = new Parser();
            Root root = parser.Parse(token);
            string syntax = root.ToString();
            Console.WriteLine(syntax);
            Checker checker = new Checker();
            checker.Check(root, Console.Error);
            Translator translator = new Translator();
            translator.Trans(root, save);
        }
    }
}
