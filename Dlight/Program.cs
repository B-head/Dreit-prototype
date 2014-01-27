using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Emit;
using Dlight.LexicalAnalysis;
using Dlight.SyntacticAnalysis;
using Dlight.Translate;

namespace Dlight
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = args[0];
            List<Element> module = new List<Element>();
            module.Add(CompileFile(fileName));
            Root root = new Root { Name = fileName.Replace(".txt", ""), Child = module };
            root.SpreadScope();
            root.CheckSemantic();
            root.CheckType();
            Console.WriteLine(root);
            if (root.ErrorCount == 0)
            {
                AssemblyTranslator trans = new AssemblyTranslator(fileName.Replace(".txt", ""));
                RegisterEmbedType(trans);
                root.SpreadTranslate(trans);
                root.Translate();
                trans.Save();
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

        static void RegisterEmbedType(AssemblyTranslator trans)
        {
            trans.RegisterEmbedType("Integer32", typeof(DlightObject.Integer32));
            trans.RegisterEmbedType("Binary64", typeof(DlightObject.Binary64));
        }
    }
}
