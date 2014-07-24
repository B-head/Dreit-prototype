using AbstractSyntax;
using AbstractSyntax.Daclate;
using CliTranslate;
using Dlight;
using NUnit.Framework;
using SyntacticAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DlightTest
{
    [TestFixture]
    class CompileTest
    {
        private static string primitive;

        [TestFixtureSetUp]
        public static void TestFixtureSetUp()
        {
            primitive = File.ReadAllText("lib/primitive.dl");
            Directory.CreateDirectory("output");
            Directory.SetCurrentDirectory("output");
        }

        [TestCaseSource(typeof(TestCaseReader))]
        public void Execute(TestData data)
        {
            Root root = new Root();
            ImportManager import = new ImportManager(root);
            root.Append(CompileText("lib/primitive.dl", primitive));
            root.Append(CompileText(data.Name, data.Code));
            root.SemanticAnalysis();
            if (root.MessageManager.MessageCount > 0)
            {
                Directory.SetCurrentDirectory("..");
                Console.WriteLine(CompileMessageBuilder.Build(root.MessageManager));
                Directory.SetCurrentDirectory("output");
            }
            Assert.That(root.MessageManager, Is.EquivalentTo(data.Message).Using<CompileMessage>(new CompileMessageEqualityComparer()));
            if (root.MessageManager.ErrorCount > 0)
            {
                Assert.That(data.ErrorCount, Is.Not.EqualTo(0), "Compile error");
                return;
            }
            if (data.NoExecute)
            {
                return;
            }
            TranslateManager trans = new TranslateManager(data.Name);
            trans.TranslateTo(root, import);
            trans.Save();
            //var cd = Directory.GetCurrentDirectory();
            //using (var process = MakeProcess(@"peverify.exe", data.Name + ".exe" + " /nologo /unique", cd))
            //{
            //    process.Start();
            //    if (!process.WaitForExit(500))
            //    {
            //        process.Kill();
            //        Assert.Fail("Peverify.exe timeout execution");
            //    }
            //    Console.WriteLine(process.StandardOutput.ReadToEnd());
            //}
            using (var process = MakeProcess(data.Name + ".exe"))
            {
                process.Start();
                process.StandardInput.WriteLine(data.Input);
                if (!process.WaitForExit(data.TimeOut ?? 500))
                {
                    process.Kill();
                    Assert.Fail(data.Name + ".exe" + " timeout execution");
                }
                var error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Assert.Fail(error);
                }
                if (data.Output != null)
                {
                    var output = TestData.CodeNormalize(process.StandardOutput.ReadToEnd(), true);
                    Assert.That(output, Is.EqualTo(data.Output));
                }
            }
        }

        private static DeclateModule CompileText(string fileName, string text)
        {
            var collection = Lexer.Lex(text, fileName);
            return Parser.Parse(collection);
        }

        private Process MakeProcess(string fileName, string arguments = "", string workingDirectory = "")
        {
            var process = new Process();
            var info = process.StartInfo;
            info.Arguments = arguments;
            info.CreateNoWindow = true;
            info.FileName = fileName;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.WorkingDirectory = workingDirectory;
            return process;
        }
    }
}
