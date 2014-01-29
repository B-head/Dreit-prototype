using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    class Root : Scope
    {
        public List<Element> Child { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }

        public Root()
        {
            Root = this;
        }

        public override int ChildCount
        {
            get { return Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return Child[index];
        }

        public override void SpreadScope(Scope scope = null, Element parent = null)
        {
            base.SpreadScope(scope, parent);
        }

        public void RegisterEmbed(Scope scope)
        {
            AddChild(scope);
            Child.Add(scope);
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
