using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DlightTest
{
    class TestCaseReader : IEnumerable<TestCaseData>
    {
        private XElement root;

        public TestCaseReader()
        {
            root = XElement.Load("BasicTest.xml", LoadOptions.None);
        }

        public IEnumerator<TestCaseData> GetEnumerator()
        {
            //int count = 0;
            XNamespace ns = "CompileTestSchema.xsd";
            foreach (var e in root.Elements(ns + "case"))
            {
                var data = new TestData
                {
                    Code = ElementToString(e.Element(ns + "code")),
                    Input = ElementToString(e.Element(ns + "input")),
                    Output = ElementToString(e.Element(ns + "output")),
                    Ignore = (bool?)e.Attribute("ignore") ?? false,
                };
                var test = new TestCaseData(data);
                if (data.Ignore)
                {
                    test.Ignore();
                }
                yield return test;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string ElementToString(XElement element)
        {
            if(element == null)
            {
                return null;
            }
            return element.Value.Trim();
        }
    }
}
