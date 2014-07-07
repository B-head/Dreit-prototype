using AbstractSyntax;
using AbstractSyntax.Daclate;
using CliTranslate;
using SyntacticAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dlight
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string fileName = args[0];
            Root root = new Root();
            ImportManager import = new ImportManager(root);
            //import.ImportAssembly(Assembly.Load("mscorlib"));
            root.AppendModule(CompileFile("lib/primitive.dl"));
            root.AppendModule(CompileFile(fileName));
            root.SemanticAnalysis();
            Console.WriteLine(CompileMessageBuilder.Build(root.MessageManager));
            if (root.MessageManager.ErrorCount > 0)
            {
                return;
            }
            TranslateManager trans = new TranslateManager(fileName.Replace(".dl", ""));
            trans.TranslateTo(root, import);
            trans.Save();
        }

        public static DeclateModule CompileFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            var collection = Lexer.Lex(text, fileName);
            string name = fileName.Replace(".dl", "").Split('/').Last();
            return Parser.Parse(collection);
        }
    }
}
