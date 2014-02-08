using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

namespace AbstractSyntax
{
    public class Root : Scope
    {
        private List<Element> _Child;
        public IReadOnlyList<Element> Child { get { return _Child; } }
        public RootTranslator RootTrans { get; private set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public Root()
        {
            _Child = new List<Element>();
            Name = string.Empty;
        }

        public void Append(Element append)
        {
            _Child.Add(append);
        }

        public override int ChildCount
        {
            get { return _Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return _Child[index];
        }

        public void SemanticAnalysis()
        {
            SpreadScope(null);
            CheckSemantic();
            CheckDataType();
        }

        public void TranslateTo(RootTranslator trans)
        {
            RootTrans = trans;
            SpreadTranslate();
            Translate();
        }

        internal void OutputError(string message)
        {
            Console.WriteLine(message);
            ErrorCount++;
        }

        internal void OutputWarning(string message)
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
