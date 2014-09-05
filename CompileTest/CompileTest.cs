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
using CliTranslate;
using Dlight;
using NUnit.Framework;
using AbstractSyntax.SyntacticAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DlightTest
{
    [TestFixture]
    class CompileTest
    {
        [TestFixtureSetUp]
        public static void TestFixtureSetUp()
        {
            Directory.CreateDirectory("output");
            if (!File.Exists(@"output/CoreLibrary.dll"))
            {
                File.Copy(@"CoreLibrary.dll", @"output\CoreLibrary.dll");
            }
            Directory.SetCurrentDirectory("output");
        }

        [TestCaseSource(typeof(TestCaseReader))]
        public void Execute(TestData data)
        {
            Root root = new Root();
            var import = new CilImport(root);
            import.ImportAssembly(Assembly.Load("mscorlib"));
            import.ImportAssembly(Assembly.Load("CoreLibrary"));
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
            var trans = SyntaxTranslator.ToStructure(root, import, data.Name);
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

        private static ModuleDeclaration CompileText(string fileName, string text)
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
