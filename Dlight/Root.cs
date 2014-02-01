using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.CilTranslate;

namespace Dlight
{
    class Root : Element
    {
        public List<Element> Child { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }

        public Root()
        {
            Child = new List<Element>();
        }

        public void Append(Element append)
        {
            Child.Add(append);
        }

        public override int ChildCount
        {
            get { return Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return Child[index];
        }

        public void PreProcess(RootTranslator trans)
        {
            SpreadScope(trans, null);
        }

        public void OutputError(string message)
        {
            Console.WriteLine(message);
            ErrorCount++;
        }

        public void OutputWarning(string message)
        {
            Console.WriteLine(message);
            WarningCount++;
        }

        public string CompileResult()
        {
            return "Error = " + ErrorCount + ", Warning = " + WarningCount;
        }

        public override string ToString(int indent)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(CompileResult());
            foreach (Element v in EnumChild())
            {
                if (v == null)
                {
                    result.AppendLine("<null>");
                    continue;
                }
                result.Append(v.ToString(indent));
            }
            return result.ToString();
        }
    }
}
