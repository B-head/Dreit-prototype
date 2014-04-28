using AbstractSyntax;
using CliTranslate;
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
                    if (!process.WaitForExit(1000))
                    {
                        process.Kill();
                        Assert.Fail("Timeout execution");
                    }
                    var error = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        Console.WriteLine(error);
                    }
                    while (true)
                    {
                        object obj = null;
                        try
                        {
                            obj = formatter.Deserialize(istream);
                        }
                        catch (SerializationException)
                        {
                            break;
                        }
                        if (obj is AssertionException)
                        {
                            var e = (AssertionException)obj;
                            throw new AssertionException(e.Message, e);
                        }
                        else if (obj is Exception)
                        {
                            throw new AggregateException("test error", (Exception)obj);
                        }
                        Console.WriteLine(obj);
                    }
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
                        Execute(data, o => formatter.Serialize(ostream, o));
                    }
                    catch (Exception e)
                    {
                        formatter.Serialize(ostream, e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        private static void Execute(TestData data, Action<object> send)
        {
            Root root = new Root();
            ImportManager import = new ImportManager(root);
            root.Append(CompileText("primitive", primitive));
            root.Append(CompileText(data.Name, data.Code));
            root.SemanticAnalysis();
            if (root.MessageManager.MessageCount > 0)
            {
                send(CompileMessageBuilder.Build(root.MessageManager));
            }
            Assert.That(root.MessageManager, Is.EquivalentTo(data.Message).Using<CompileMessage>(new CompileMessageEqualityComparer()));
            if (root.MessageManager.ErrorCount > 0)
            {
                Assert.That(data.ErrorCount, Is.Not.EqualTo(0), "Compile error");
                return;
            }
            if(data.NoExecute)
            {
                return;
            }
            TranslateManager trans = new TranslateManager(data.Name);
            trans.TranslateTo(root, import);
            trans.Run();
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
