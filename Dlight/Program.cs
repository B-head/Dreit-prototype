using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dlight.LexicalAnalysis;

namespace Dlight
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(args[0]);
            Lexer lex = new Lexer();
            List<Token> token = lex.Lex(text, args[0]);
            foreach(Token v in token)
            {
                Console.WriteLine(v.ToString());
            }
        }
    }
}
