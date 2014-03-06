using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;

namespace AbstractSyntax
{
    public class Root : NameSpace
    {
        public RootTranslator RootTrans { get; private set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public Root()
        {
            Name = "global";
        }

        public void SemanticAnalysis()
        {
            SpreadElement(null, null);
            SpreadReference(null);
            CheckSyntax();
            CheckDataType(null);
        }

        public void TranslateTo(RootTranslator trans)
        {
            RootTrans = trans;
            PreSpreadTranslate(RootTrans);
            PostSpreadTranslate(RootTrans);
            Translate(trans);
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
    }
}
