using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax
{
    [Serializable]
    public class Root : NameSpaceSymbol
    {
        private NameSpaceSymbol BuiltInList;
        public CompileMessageManager MessageManager { get; private set; }
        internal TypeManager TypeManager { get; private set; }
        internal ConversionManager ConvManager { get; private set; }
        internal OperationManager OpManager { get; private set; }
        internal OverLoadReference UndefinedOverLord { get; private set; }
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
            BuiltInList = new NameSpaceSymbol();
            UndefinedOverLord = new OverLoadReference(this, null);
            TypeManager = new TypeManager();
            ConvManager = new ConversionManager(this);
            OpManager = new OperationManager(this);
            MessageManager = new CompileMessageManager();
            CreatePragma();
            CreateBuiltInIdentifier();
            AppendChild(BuiltInList);
            AppendChild(TypeManager);
        }

        public void AppendModule(NameSpaceSymbol ns)
        {
            AppendChild(ns);
        }

        public void SemanticAnalysis()
        {
            TraversalCheckSemantic(this);
        }

        private void TraversalCheckSemantic(Element element)
        {
            element.CheckSemantic(MessageManager);
            foreach(var v in element)
            {
                TraversalCheckSemantic(v);
            }
        }

        private void CreatePragma()
        {
            BuiltInList.AppendChild(new CastPragma());
            BuiltInList.AppendChild(new CalculatePragma("add", CalculatePragmaType.Add));
            BuiltInList.AppendChild(new CalculatePragma("sub", CalculatePragmaType.Sub));
            BuiltInList.AppendChild(new CalculatePragma("mul", CalculatePragmaType.Mul));
            BuiltInList.AppendChild(new CalculatePragma("div", CalculatePragmaType.Div));
            BuiltInList.AppendChild(new CalculatePragma("mod", CalculatePragmaType.Mod));
            BuiltInList.AppendChild(new CalculatePragma("eq", CalculatePragmaType.EQ));
            BuiltInList.AppendChild(new CalculatePragma("ne", CalculatePragmaType.NE));
            BuiltInList.AppendChild(new CalculatePragma("lt", CalculatePragmaType.LT));
            BuiltInList.AppendChild(new CalculatePragma("le", CalculatePragmaType.LE));
            BuiltInList.AppendChild(new CalculatePragma("gt", CalculatePragmaType.GT));
            BuiltInList.AppendChild(new CalculatePragma("ge", CalculatePragmaType.GE));
            BuiltInList.AppendChild(new PrimitivePragma("Object", PrimitivePragmaType.Object));
            BuiltInList.AppendChild(new PrimitivePragma("String", PrimitivePragmaType.String));
            BuiltInList.AppendChild(new PrimitivePragma("Boolean", PrimitivePragmaType.Boolean));
            BuiltInList.AppendChild(new PrimitivePragma("Integer8", PrimitivePragmaType.Integer8));
            BuiltInList.AppendChild(new PrimitivePragma("Integer16", PrimitivePragmaType.Integer16));
            BuiltInList.AppendChild(new PrimitivePragma("Integer32", PrimitivePragmaType.Integer32));
            BuiltInList.AppendChild(new PrimitivePragma("Integer64", PrimitivePragmaType.Integer64));
            BuiltInList.AppendChild(new PrimitivePragma("Natural8", PrimitivePragmaType.Natural8));
            BuiltInList.AppendChild(new PrimitivePragma("Natural16", PrimitivePragmaType.Natural16));
            BuiltInList.AppendChild(new PrimitivePragma("Natural32", PrimitivePragmaType.Natural32));
            BuiltInList.AppendChild(new PrimitivePragma("Natural64", PrimitivePragmaType.Natural64));
            BuiltInList.AppendChild(new PrimitivePragma("Binary32", PrimitivePragmaType.Binary32));
            BuiltInList.AppendChild(new PrimitivePragma("Binary64", PrimitivePragmaType.Binary64));
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
            BuiltInList.AppendChild(Void);
            BuiltInList.AppendChild(Unknown);
            BuiltInList.AppendChild(Error);
            BuiltInList.AppendChild(Var);
            BuiltInList.AppendChild(Let);
            BuiltInList.AppendChild(Routine);
            BuiltInList.AppendChild(Function);
            BuiltInList.AppendChild(Class);
            BuiltInList.AppendChild(Trait);
            BuiltInList.AppendChild(Refer);
            BuiltInList.AppendChild(Typeof);
            BuiltInList.AppendChild(new BooleanSymbol(false));
            BuiltInList.AppendChild(new BooleanSymbol(true));
            BuiltInList.AppendChild(new AttributeSymbol("static", AttributeType.Static));
            BuiltInList.AppendChild(new AttributeSymbol("public", AttributeType.Public));
            BuiltInList.AppendChild(new AttributeSymbol("protected", AttributeType.Protected));
            BuiltInList.AppendChild(new AttributeSymbol("private", AttributeType.Private));
        }
    }
}
