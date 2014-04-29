using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class Root : NameSpace
    {
        private Dictionary<string, OverLoadScope> PragmaDictionary;
        private DirectiveList PragmaList;
        public CompileMessageManager MessageManager { get; private set; }
        public VoidSymbol Void { get; private set; }
        public OverLoadScope VoidOverLoad { get; private set; }

        public Root()
        {
            Name = "global";
            PragmaList = new DirectiveList();
            PragmaDictionary = new Dictionary<string, OverLoadScope>();
            MessageManager = new CompileMessageManager();
            Void = new VoidSymbol();
            VoidOverLoad = new OverLoadScope(Void);
            CreatePragma();
        }

        public override int Count
        {
            get { return 3; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ExpList;
                    case 1: return PragmaList;
                    case 2: return Void;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SemanticAnalysis()
        {
            SpreadElement(null, null);
            CheckSyntax();
            CheckDataType();
        }

        internal OverLoadScope GetPragma(string name)
        {
            OverLoadScope temp;
            PragmaDictionary.TryGetValue(name, out temp);
            return temp;
        }

        private void AppendPragma(string name, Scope pragma)
        {
            pragma.Name = "@@" + name;
            PragmaList.Append(pragma);
            var ol = new OverLoadScope();
            ol.Append(pragma);
            PragmaDictionary.Add(name, ol);
        }

        private void CreatePragma()
        {
            AppendPragma("add", new CalculatePragma(CalculatePragmaType.Add));
            AppendPragma("sub", new CalculatePragma(CalculatePragmaType.Sub));
            AppendPragma("mul", new CalculatePragma(CalculatePragmaType.Mul));
            AppendPragma("div", new CalculatePragma(CalculatePragmaType.Div));
            AppendPragma("mod", new CalculatePragma(CalculatePragmaType.Mod));
            AppendPragma("cast", new CastPragma());
            AppendPragma("Object", new PrimitivePragma(PrimitivePragmaType.Object));
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
    }
}
