using AbstractSyntax;
using CliTranslate;
using NUnit.Framework;
using SyntacticAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;

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
        public void Test(TestData data)
        {
            using (AnonymousPipeServerStream
                ostream = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable),
                istream = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            {
                var arguments = ostream.GetClientHandleAsString() + " " + istream.GetClientHandleAsString();
                var formatter = new BinaryFormatter();
                using (var process = MakeProcess("DlightTest.exe", arguments))
                {
                    process.Start();
                    ostream.DisposeLocalCopyOfClientHandle();
                    istream.DisposeLocalCopyOfClientHandle();
                    formatter.Serialize(ostream, data);
                    process.StandardInput.WriteLine(data.Input);
                    if (!process.WaitForExit(5000))
                    {
                        process.Kill();
                        process.WaitForExit();
                        Assert.Fail("Timeout execution");
                    }
                    var error = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        Console.WriteLine(error);
                    }
                    object obj = formatter.Deserialize(istream);
                    if (obj is Exception)
                    {
                        throw new AggregateException("test failed", (Exception)obj);
                    }
                    Console.WriteLine(obj);
                    if (data.Output != null)
                    {
                        var output = TestData.CodeNormalize(process.StandardOutput.ReadToEnd());
                        Assert.That(output, Is.EqualTo(data.Output));
                    }
                }
            }
        }

        private Process MakeProcess(string fileName, string arguments)
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
            return process;
        }

        public static void Main(string[] args)
        {
            try
            {
                using (AnonymousPipeClientStream
                    istream = new AnonymousPipeClientStream(PipeDirection.In, args[0]),
                    ostream = new AnonymousPipeClientStream(PipeDirection.Out, args[1]))
                {
                    var formatter = new BinaryFormatter();
                    TestData data = formatter.Deserialize(istream) as TestData;
                    try
                    {
                        var message = Execute(data);
                        formatter.Serialize(ostream, message);
                    }
                    catch (Exception e)
                    {
                        formatter.Serialize(ostream, e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static string Execute(TestData data)
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
                Assert.That(data.ErrorCount, Is.Not.EqualTo(0), "Compile error");
                return message;
            }
            if(data.NoExecute)
            {
                return message;
            }
            TranslateManager trans = new TranslateManager(data.Name);
            trans.TranslateTo(root, import);
            trans.Run();
            return message;
        }

        private static Element CompileText(string name, string text)
        {
            List<Token> tokenList, errorToken;
            TextPosition position;
            Lexer.Lex(text, name, out tokenList, out errorToken, out position);
            Parser parser = new Parser();
            return parser.Parse(text, name, tokenList, errorToken, position);
        }
    }
}
