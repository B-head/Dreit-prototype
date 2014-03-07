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
        private Dictionary<string, Pragma> _PragmaDictionary;
        public IReadOnlyDictionary<string, Pragma> PragmaDictionary { get; set; }

        public Root()
        {
            Name = "global";
            _PragmaDictionary = new Dictionary<string, Pragma>();
            CreatePragma();
        }

        public void SemanticAnalysis()
        {
            SpreadElement(null, null);
            SpreadReference(null);
            CheckSyntax();
            CheckDataType();
        }

        public void TranslateTo(RootTranslator trans)
        {
            RootTrans = trans;
            PreSpreadTranslate(RootTrans);
            PostSpreadTranslate(RootTrans);
            Translate(trans);
        }

        internal Pragma GetPragma(string name)
        {
            Pragma temp;
            _PragmaDictionary.TryGetValue(name, out temp);
            return temp;
        }

        private void CreatePragma()
        {
            _PragmaDictionary.Add("add", new CalculatePragma(CalculatePragmaType.Add));
            _PragmaDictionary.Add("sub", new CalculatePragma(CalculatePragmaType.Sub));
            _PragmaDictionary.Add("mul", new CalculatePragma(CalculatePragmaType.Mul));
            _PragmaDictionary.Add("div", new CalculatePragma(CalculatePragmaType.Div));
            _PragmaDictionary.Add("mod", new CalculatePragma(CalculatePragmaType.Mod));
            _PragmaDictionary.Add("Root", new PrimitivePragma(PrimitivePragmaType.Root));
            _PragmaDictionary.Add("Integer32", new PrimitivePragma(PrimitivePragmaType.Integer32));
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
