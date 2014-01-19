using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dlight.LexicalAnalysis;
using Dlight.SyntacticAnalysis;

namespace Dlight
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = args[0];
            string text = File.ReadAllText(file);
            Lexer lexer = new Lexer();
            List<Token> token = lexer.Lex(text, file);
            Parser parser = new Parser();
            Syntax syntax = parser.Parse(token);
            string output = syntax.ToString();
            Console.WriteLine(output);
        }
    }
}
