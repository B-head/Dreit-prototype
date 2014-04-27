using AbstractSyntax;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DlightTest
{
    class TestCaseReader : IEnumerable<TestCaseData>
    {
        private static List<TestData> testData;
        private static readonly XNamespace ns = "CompileTestSchema.xsd";

        static TestCaseReader()
        {
            testData = new List<TestData>();
            foreach (var file in Directory.EnumerateFiles(@"./", "*.xml"))
            {
                var element = XElement.Load(file);
                if (element.Name != ns + "compile-test")
                {
                    continue;
                }
                AppendData(file.Replace(".xml", "").Split('/').Last(), element);
            }
        }

        private static void AppendData(string category, XElement element)
        {
            int count = 0;
            foreach (var e in element.Descendants(ns + "case"))
            {
                ++count;
                var message = new List<CompileMessage>();
                var ic = AppendMessage(message, e, "info", CompileMessageType.Info);
                var ec = AppendMessage(message, e, "error", CompileMessageType.Error);
                var wc =AppendMessage(message, e, "warning", CompileMessageType.Warning);
                var data = new TestData
                {
                    Name = category + "-" + ((string)e.Attribute("name") ?? count.ToString()),
                    Category = category,
                    Ignore = (bool?)e.Attribute("ignore") ?? false,
                    Explicit = (bool?)e.Attribute("explicit") ?? false,
                    Code = CodeNormalize(e.Element(ns + "code")),
                    Message = message,
                    InfoCount = ic,
                    ErrorCount = ec,
                    WarningCount = wc,
                    Input = CodeNormalize(e.Element(ns + "input")),
                    Output = CodeNormalize(e.Element(ns + "output")),
                };
                testData.Add(data);
            }
        }

        private static int AppendMessage(List<CompileMessage> list, XElement element, string name, CompileMessageType type)
        {
            int count = 0;
            foreach(var e in element.Elements(ns + name))
            {
                ++count;
                var m = new CompileMessage
                {
                    Key = (string)e,
                    Type = type,
                };
                list.Add(m);
            }
            return count;
        }

        private static string CodeNormalize(XElement element)
        {
            if (element == null)
            {
                return null;
            }
            var temp = element.Value.Trim();
            return Regex.Replace(temp, @"\s+", " ");
        }

        public IEnumerator<TestCaseData> GetEnumerator()
        {
            foreach (var data in testData)
            {
                var test = new TestCaseData(data);
                test.SetName(data.Name + "(" + data.Code + ")");
                test.SetCategory(data.Category);
                if (data.Ignore)
                {
                    test.Ignore();
                }
                if (data.Explicit)
                {
                    test.MakeExplicit();
                }
                yield return test;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
