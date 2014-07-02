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
        internal OverLoad UndefinedOverLord { get; private set; }
        internal ConversionManager Conversion { get; set; }
        internal Dictionary<TokenType, ConversionManager> OpManager { get; set; }
        public CompileMessageManager MessageManager { get; private set; }

        public Root()
        {
            Name = "global";
            BuiltInList = new DirectiveList();
            Void = new VoidSymbol() { Name = "void" };
            Error = new ErrorSymbol();
            Unknown = new UnknownSymbol();
            UndefinedOverLord = new OverLoad(Unknown, true);
            Conversion = new ConversionManager(Void, Error, Unknown);
            MessageManager = new CompileMessageManager();
            CreatePragma();
            CreateBuiltInIdentifier();
            CreateOperatorManager();
        }

        public override int Count
        {
            get { return 2; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ExpList;
                    case 1: return BuiltInList;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SemanticAnalysis()
        {
            SpreadElement(null, null);
            CheckSemantic();
        }

        private void AppendPragma(string name, Scope pragma)
        {
            pragma.Name = "@@" + name;
            BuiltInList.Append(pragma);
        }

        private void CreatePragma()
        {
            AppendPragma("cast", new CastPragma());
            AppendPragma("add", new CalculatePragma(CalculatePragmaType.Add));
            AppendPragma("sub", new CalculatePragma(CalculatePragmaType.Sub));
            AppendPragma("mul", new CalculatePragma(CalculatePragmaType.Mul));
            AppendPragma("div", new CalculatePragma(CalculatePragmaType.Div));
            AppendPragma("mod", new CalculatePragma(CalculatePragmaType.Mod));
            AppendPragma("eq", new CalculatePragma(CalculatePragmaType.Eq));
            AppendPragma("ne", new CalculatePragma(CalculatePragmaType.Ne));
            AppendPragma("lt", new CalculatePragma(CalculatePragmaType.Lt));
            AppendPragma("le", new CalculatePragma(CalculatePragmaType.Le));
            AppendPragma("gt", new CalculatePragma(CalculatePragmaType.Gt));
            AppendPragma("ge", new CalculatePragma(CalculatePragmaType.Ge));
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
            BuiltInList.Append(Unknown);
            BuiltInList.Append(Error);
            BuiltInList.Append(new BooleanSymbol(false) { Name = "false" });
            BuiltInList.Append(new BooleanSymbol(true) { Name = "true" });
            BuiltInList.Append(new AttributeSymbol(AttributeType.Static) { Name = "static" });
            BuiltInList.Append(new AttributeSymbol(AttributeType.Public) { Name = "public" });
            BuiltInList.Append(new AttributeSymbol(AttributeType.Protected) { Name = "protected" });
            BuiltInList.Append(new AttributeSymbol(AttributeType.Private) { Name = "private" });
        }

        private void CreateOperatorManager()
        {
            OpManager = new Dictionary<TokenType, ConversionManager>();
            OpManager.Add(TokenType.Add, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Subtract, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Multiply, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Divide, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Modulo, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Equal, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.NotEqual, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.LessThan, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.LessThanOrEqual, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.GreaterThan, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.GreaterThanOrEqual, new ConversionManager(Void, Error, Unknown));
            OpManager.Add(TokenType.Incompare, new ConversionManager(Void, Error, Unknown));
        }
    }
}
