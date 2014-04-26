using AbstractSyntax;
using CliTranslate;
using NUnit.Framework;
using SyntacticAnalysis;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace DlightTest
{
    [TestFixture]
    class CompileTest
    {
        private string primitive;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            primitive = File.ReadAllText("lib/primitive.dl");
        }

        [TestCaseSource(typeof(TestCaseReader))]
        public void Execute(TestData data)
        {
            Root root = new Root();
            ImportManager import = new ImportManager(root);
            root.Append(CompileText("primitive", primitive));
            root.Append(CompileText("test", data.Code));
            root.SemanticAnalysis();
            if (root.CompileInfo.ErrorCount > 0)
            {
                Console.WriteLine(root.CompileInfo);
                Assert.Fail("Compile error");
            }
            TranslateManager trans = new TranslateManager("test");
            trans.TranslateTo(root, import);
            trans.Save();
            using (var process = MakeProcess("test.exe"))
            {
                process.Start();
                process.StandardInput.WriteLine(data.Input);
                if (process.WaitForExit(1000))
                {
                    var output = process.StandardOutput.ReadToEnd().Trim();
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
