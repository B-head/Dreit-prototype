using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public class Root : NameSpace
    {
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }
        private List<Scope> PragmaList;
        private Dictionary<string, Scope> _PragmaDictionary;
        public IReadOnlyDictionary<string, Scope> PragmaDictionary { get; set; }

        public Root()
        {
            Name = "global";
            PragmaList = new List<Scope>();
            _PragmaDictionary = new Dictionary<string, Scope>();
            CreatePragma();
        }

        public override int Count
        {
            get { return Child.Count + PragmaList.Count; }
        }

        public override Element GetChild(int index)
        {
            if (index < Child.Count)
            {
                return Child[index];
            }
            else
            {
                return PragmaList[index - Child.Count];
            }
        }

        public void SemanticAnalysis()
        {
            SpreadElement(null, null);
            SpreadReference(null);
            CheckSyntax();
            CheckDataType();
        }

        internal Scope GetPragma(string name)
        {
            Scope temp;
            _PragmaDictionary.TryGetValue(name, out temp);
            return temp;
        }

        private void AppendPragma(string name, Scope pragma)
        {
            pragma.Name = "@@" + name;
            PragmaList.Add(pragma);
            _PragmaDictionary.Add(name, pragma);
        }

        private void CreatePragma()
        {
            AppendPragma("add", new CalculatePragma(CalculatePragmaType.Add));
            AppendPragma("sub", new CalculatePragma(CalculatePragmaType.Sub));
            AppendPragma("mul", new CalculatePragma(CalculatePragmaType.Mul));
            AppendPragma("div", new CalculatePragma(CalculatePragmaType.Div));
            AppendPragma("mod", new CalculatePragma(CalculatePragmaType.Mod));
            AppendPragma("cast", new CalculatePragma(CalculatePragmaType.Cast));
            AppendPragma("Root", new PrimitivePragma(PrimitivePragmaType.Root));
            AppendPragma("Boolean", new PrimitivePragma(PrimitivePragmaType.Boolean));
            AppendPragma("Integer8", new PrimitivePragma(PrimitivePragmaType.Integer8));
            AppendPragma("Integer16", new PrimitivePragma(PrimitivePragmaType.Integer16));
            AppendPragma("Integer32", new PrimitivePragma(PrimitivePragmaType.Integer32));
            AppendPragma("Integer64", new PrimitivePragma(PrimitivePragmaType.Integer64));
            AppendPragma("Natural8", new PrimitivePragma(PrimitivePragmaType.Natural8));
            AppendPragma("Natural16", new PrimitivePragma(PrimitivePragmaType.Natural16));
            AppendPragma("Natural32", new PrimitivePragma(PrimitivePragmaType.Natural32));
            AppendPragma("Natural64", new PrimitivePragma(PrimitivePragmaType.Natural64));
            AppendPragma("Binary32", new PrimitivePragma(PrimitivePragmaType.Binary32));
            AppendPragma("Binary64", new PrimitivePragma(PrimitivePragmaType.Binary64));
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
