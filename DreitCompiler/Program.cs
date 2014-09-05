/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using AbstractSyntax;
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

        public static void CopyLibrary(string name)
        {
            var assembly = Assembly.GetEntryAssembly();
            var assdir = Path.GetDirectoryName(assembly.Location);
            var curdir = Directory.GetCurrentDirectory();
            if (assdir == curdir) 
            {
                return;
            }
            File.Copy(assdir + name, curdir + name, true);
        }
    }
}
