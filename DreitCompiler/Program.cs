﻿using AbstractSyntax;
using AbstractSyntax.Declaration;
using AbstractSyntax.SyntacticAnalysis;
using AbstractSyntax.Visualizer;
using CliTranslate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dlight
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string fileName = args[0];
            var root = new Root();
            var import = new CilImport(root);
            import.ImportAssembly(Assembly.Load("mscorlib"));
            import.ImportAssembly(Assembly.Load("CoreLibrary"));
            root.Append(CompileFile(fileName));
            root.SemanticAnalysis();
            Console.WriteLine(CompileMessageBuilder.Build(root.MessageManager));
            if (root.MessageManager.ErrorCount > 0)
            {
                return;
            }
            var trans = SyntaxTranslator.ToStructure(root, import, fileName.Replace(".dr", ""));
            trans.Save();
        }

        public static ModuleDeclaration CompileFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            var collection = Lexer.Lex(text, fileName);
            string name = fileName.Replace(".dr", "").Split('/').Last();
            return Parser.Parse(collection);
        }
    }
}
