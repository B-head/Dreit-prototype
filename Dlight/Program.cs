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
            Root root = new Root { Child = module };
            RegisterEmbed(root);
            root.SpreadScope();
            root.CheckSemantic();
            root.CheckDataType();
            Console.WriteLine(root);
            if (root.ErrorCount == 0)
            {
                AssemblyTranslator trans = new AssemblyTranslator(fileName.Replace(".txt", ""));
                RegisterEmbed(trans);
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

        static void RegisterEmbed(Root root)
        {
            TextPosition p = new TextPosition { File = "@@Embed" };
            root.RegisterEmbed(new Scope { Name = "Integer32", Position = p });
            root.RegisterEmbed(new Scope { Name = "Binary64", Position = p });
        }

        static void RegisterEmbed(AssemblyTranslator trans)
        {
            trans.RegisterEmbed("Integer32", typeof(DlightObject.Integer32));
            trans.RegisterEmbed("Binary64", typeof(DlightObject.Binary64));
        }
    }
}
