using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(args[0]);
            LexicalAnalysis lex = new LexicalAnalysis(text, args[0]);
            foreach(Token v in lex)
            {
                Console.WriteLine(v.ToString());
            }
        }
    }
}
