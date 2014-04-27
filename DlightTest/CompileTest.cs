using AbstractSyntax;
using CliTranslate;
using NUnit.Framework;
using SyntacticAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DlightTest
{
    [TestFixture]
    class CompileTest
    {
        private static string primitive;

        static CompileTest()
        {
            primitive = File.ReadAllText("lib/primitive.dl");
        }

        [TestCaseSource(typeof(TestCaseReader))]
        public void Execute(TestData data)
        {
            Root root = new Root();
            ImportManager import = new ImportManager(root);
            root.Append(CompileText("primitive", primitive));
            root.Append(CompileText(data.Name, data.Code));
            root.SemanticAnalysis();
            Assert.That(root.MessageManager, Is.EquivalentTo(data.Message).Using<CompileMessage>(new CompileMessageEqualityComparer()));
            var message = CompileMessageBuilder.Build(root.MessageManager);
            if (root.MessageManager.ErrorCount > 0)
            {
                Console.WriteLine(message);
                Assert.That(data.ErrorCount, Is.Not.EqualTo(0), "Compile error");
                return;
            }
            if(data.NoExecute)
            {
                return;
            }
            TranslateManager trans = new TranslateManager(data.Name);
            trans.TranslateTo(root, import);
            trans.Save();
            using (var process = MakeProcess(data.Name + ".exe"))
            {
                process.Start();
                process.StandardInput.WriteLine(data.Input);
                if (process.WaitForExit(1000))
                {
                    var output = TestData.CodeNormalize(process.StandardOutput.ReadToEnd());
                    Assert.That(output, Is.EqualTo(data.Output));
                }
                else
                {
                    process.Kill();
                    process.WaitForExit();
                    Assert.Fail("Timeout execution");
                }
            }
        }

        public Element CompileText(string name, string text)
        {
            List<Token> tokenList, errorToken;
            TextPosition position;
            Lexer.Lex(text, name, out tokenList, out errorToken, out position);
            Parser parser = new Parser();
            return parser.Parse(text, name, tokenList, errorToken, position);
        }

        public Process MakeProcess(string fileName)
        {
            var process = new Process();
            var info = process.StartInfo;
            info.CreateNoWindow = true;
            info.FileName = fileName;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            return process;
        }
    }
}
