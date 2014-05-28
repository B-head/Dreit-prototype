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
        private DirectiveList BuiltInList;
        internal VoidSymbol Void { get; private set; }
        internal ErrorSymbol Error { get; private set; }
        internal UnknownSymbol Unknown { get; private set; }
        internal UnknownOverLoad UnknownOverLoad { get; private set; }
        internal ConversionManager Conversion { get; set; }
        internal Dictionary<TokenType, ConversionManager> OpManager { get; set; }
        public CompileMessageManager MessageManager { get; private set; }

        public Root()
        {
            Name = "global";
            BuiltInList = new DirectiveList();
            Void = new VoidSymbol();
            Error = new ErrorSymbol();
            Unknown = new UnknownSymbol();
            UnknownOverLoad = new UnknownOverLoad(Unknown);
            Conversion = new ConversionManager(Void, Error, Unknown);
            MessageManager = new CompileMessageManager();
            CreatePragma();
            CreateBuiltInIdentifier();
            CreateOperatorManager();
        }

        public override int Count
        {
            get { return 4; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ExpList;
                    case 1: return BuiltInList;
                    case 2: return Error;
                    case 3: return Unknown;
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

        private void AppendPragma(string name, Scope pragma)
        {
            pragma.Name = "@@" + name;
            BuiltInList.Append(pragma);
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

        private void CreateBuiltInIdentifier()
        {
            BuiltInList.Append(Void);
            BuiltInList.Append(new BooleanSymbol(false) { Name = "false" });
            BuiltInList.Append(new BooleanSymbol(true) { Name = "true" });
        }

        private void CreateOperatorManager()
        {
            OpManager = new Dictionary<TokenType, ConversionManager>();
            OpManager.Add(TokenType.Add, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Subtract, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Multiply, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Divide, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Modulo, new ConversionManager(Void, Error, Unknown));
        }
    }
}
