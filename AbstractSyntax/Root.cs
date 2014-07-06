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
        public CompileMessageManager MessageManager { get; private set; }
        internal TypeManager TypeManager { get; private set; }
        internal ConversionManager Conversion { get; private set; }
        internal Dictionary<TokenType, ConversionManager> OpManager { get; private set; }
        internal OverLoad UndefinedOverLord { get; private set; }
        internal VoidSymbol Void { get; private set; }
        internal ErrorSymbol Error { get; private set; }
        internal UnknownSymbol Unknown { get; private set; }
        internal AttributeSymbol Var { get; private set; }
        internal AttributeSymbol Let { get; private set; }
        internal AttributeSymbol Routine { get; private set; }
        internal AttributeSymbol Function { get; private set; }
        internal AttributeSymbol Class { get; private set; }
        internal AttributeSymbol Trait { get; private set; }
        internal AttributeSymbol Refer { get; private set; }
        internal AttributeSymbol Typeof { get; private set; }

        public Root()
        {
            Name = "global";
            BuiltInList = new DirectiveList();
            UndefinedOverLord = new OverLoad(this, true);
            TypeManager = new TypeManager(this);
            Conversion = new ConversionManager(this);
            MessageManager = new CompileMessageManager();
            CreatePragma();
            CreateBuiltInIdentifier();
            CreateOperatorManager();
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Element this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ExpList;
                    case 1: return BuiltInList;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public void SemanticAnalysis()
        {
            SpreadElement(null, null);
            CheckSemantic();
        }

        private void CreatePragma()
        {
            BuiltInList.Append(new CastPragma());
            BuiltInList.Append(new CalculatePragma("add", CalculatePragmaType.Add));
            BuiltInList.Append(new CalculatePragma("sub", CalculatePragmaType.Sub));
            BuiltInList.Append(new CalculatePragma("mul", CalculatePragmaType.Mul));
            BuiltInList.Append(new CalculatePragma("div", CalculatePragmaType.Div));
            BuiltInList.Append(new CalculatePragma("mod", CalculatePragmaType.Mod));
            BuiltInList.Append(new CalculatePragma("eq", CalculatePragmaType.EQ));
            BuiltInList.Append(new CalculatePragma("ne", CalculatePragmaType.NE));
            BuiltInList.Append(new CalculatePragma("lt", CalculatePragmaType.LT));
            BuiltInList.Append(new CalculatePragma("le", CalculatePragmaType.LE));
            BuiltInList.Append(new CalculatePragma("gt", CalculatePragmaType.GT));
            BuiltInList.Append(new CalculatePragma("ge", CalculatePragmaType.GE));
            BuiltInList.Append(new PrimitivePragma("Object", PrimitivePragmaType.Object));
            BuiltInList.Append(new PrimitivePragma("String", PrimitivePragmaType.String));
            BuiltInList.Append(new PrimitivePragma("Boolean", PrimitivePragmaType.Boolean));
            BuiltInList.Append(new PrimitivePragma("Integer8", PrimitivePragmaType.Integer8));
            BuiltInList.Append(new PrimitivePragma("Integer16", PrimitivePragmaType.Integer16));
            BuiltInList.Append(new PrimitivePragma("Integer32", PrimitivePragmaType.Integer32));
            BuiltInList.Append(new PrimitivePragma("Integer64", PrimitivePragmaType.Integer64));
            BuiltInList.Append(new PrimitivePragma("Natural8", PrimitivePragmaType.Natural8));
            BuiltInList.Append(new PrimitivePragma("Natural16", PrimitivePragmaType.Natural16));
            BuiltInList.Append(new PrimitivePragma("Natural32", PrimitivePragmaType.Natural32));
            BuiltInList.Append(new PrimitivePragma("Natural64", PrimitivePragmaType.Natural64));
            BuiltInList.Append(new PrimitivePragma("Binary32", PrimitivePragmaType.Binary32));
            BuiltInList.Append(new PrimitivePragma("Binary64", PrimitivePragmaType.Binary64));
        }

        private void CreateBuiltInIdentifier()
        {
            Void = new VoidSymbol();
            Error = new ErrorSymbol();
            Unknown = new UnknownSymbol();
            Var = new AttributeSymbol(AttributeType.Var);
            Let = new AttributeSymbol(AttributeType.Let);
            Routine = new AttributeSymbol(AttributeType.Routine);
            Function = new AttributeSymbol(AttributeType.Function);
            Class = new AttributeSymbol(AttributeType.Class);
            Trait = new AttributeSymbol(AttributeType.Trait);
            Refer = new AttributeSymbol(AttributeType.Refer);
            Typeof = new AttributeSymbol(AttributeType.Tyoeof);
            BuiltInList.Append(Void);
            BuiltInList.Append(Unknown);
            BuiltInList.Append(Error);
            BuiltInList.Append(Var);
            BuiltInList.Append(Let);
            BuiltInList.Append(Routine);
            BuiltInList.Append(Function);
            BuiltInList.Append(Class);
            BuiltInList.Append(Trait);
            BuiltInList.Append(Refer);
            BuiltInList.Append(Typeof);
            BuiltInList.Append(new BooleanSymbol(false));
            BuiltInList.Append(new BooleanSymbol(true));
            BuiltInList.Append(new AttributeSymbol("static", AttributeType.Static));
            BuiltInList.Append(new AttributeSymbol("public", AttributeType.Public));
            BuiltInList.Append(new AttributeSymbol("protected", AttributeType.Protected));
            BuiltInList.Append(new AttributeSymbol("private", AttributeType.Private));
        }

        private void CreateOperatorManager()
        {
            OpManager = new Dictionary<TokenType, ConversionManager>();
            OpManager.Add(TokenType.Add, new ConversionManager(this));
            OpManager.Add(TokenType.Subtract, new ConversionManager(this));
            OpManager.Add(TokenType.Multiply, new ConversionManager(this));
            OpManager.Add(TokenType.Divide, new ConversionManager(this));
            OpManager.Add(TokenType.Modulo, new ConversionManager(this));
            OpManager.Add(TokenType.Equal, new ConversionManager(this));
            OpManager.Add(TokenType.NotEqual, new ConversionManager(this));
            OpManager.Add(TokenType.LessThan, new ConversionManager(this));
            OpManager.Add(TokenType.LessThanOrEqual, new ConversionManager(this));
            OpManager.Add(TokenType.GreaterThan, new ConversionManager(this));
            OpManager.Add(TokenType.GreaterThanOrEqual, new ConversionManager(this));
            OpManager.Add(TokenType.Incompare, new ConversionManager(this));
        }
    }
}
