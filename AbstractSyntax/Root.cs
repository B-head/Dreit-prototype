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
        public VoidSymbol Void { get; private set; }
        internal ErrorSymbol Error { get; private set; }
        internal UnknownSymbol Unknown { get; private set; }
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
            CreateBuildInOperator();
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
            Refer = new AttributeSymbol(AttributeType.Refer);
            Typeof = new AttributeSymbol(AttributeType.Tyoeof);
            BuiltInList.AppendChild(Void);
            BuiltInList.AppendChild(Unknown);
            BuiltInList.AppendChild(Error);
            BuiltInList.AppendChild(Refer);
            BuiltInList.AppendChild(Typeof);
            BuiltInList.AppendChild(new BooleanSymbol(false));
            BuiltInList.AppendChild(new BooleanSymbol(true));
            BuiltInList.AppendChild(new AttributeSymbol("static", AttributeType.Static));
            BuiltInList.AppendChild(new AttributeSymbol("public", AttributeType.Public));
            BuiltInList.AppendChild(new AttributeSymbol("protected", AttributeType.Protected));
            BuiltInList.AppendChild(new AttributeSymbol("private", AttributeType.Private));
        }

        private void CreateBuildInOperator()
        {
            var nt = GetBuildInNumberType();
            CreateBuiltInCast(nt);
            CreateBuiltInMonadicOperator(nt);
            CreateBuiltInDyadicOperator(nt);
        }

        private void CreateBuiltInCast(IReadOnlyDictionary<ClassSymbol, PrimitivePragmaType> nt)
        {
            foreach(var a in nt.Keys)
            {
                foreach(var b in nt.Keys)
                {
                    if(a == b)
                    {
                        continue;
                    }
                    var p = new CastPragma(nt[b], a, b);
                    BuiltInList.AppendChild(p);
                    ConvManager.Append(p);
                }
            }
        }

        private void CreateBuiltInMonadicOperator(IReadOnlyDictionary<ClassSymbol, PrimitivePragmaType> nt)
        {
            var bl = (ClassSymbol)NameResolution("Boolean").FindDataType();
            foreach (var a in nt.Keys)
            {
                foreach (MonadicOperatorPragmaType t in Enum.GetValues(typeof(MonadicOperatorPragmaType)))
                {
                    MonadicOperatorPragma p;
                    if (MonadicOperatorPragma.HasCondition(t))
                    {
                        p = new MonadicOperatorPragma(t, a, bl);
                    }
                    else
                    {
                        p = new MonadicOperatorPragma(t, a, a);
                    }
                    BuiltInList.AppendChild(p);
                    OpManager.Append(p);
                }
            }
        }

        private void CreateBuiltInDyadicOperator(IReadOnlyDictionary<ClassSymbol, PrimitivePragmaType> nt)
        {
            var bl = (ClassSymbol)NameResolution("Boolean").FindDataType();
            foreach (var a in nt.Keys)
            {
                foreach (var b in nt.Keys)
                {
                    foreach (DyadicOperatorPragmaType t in Enum.GetValues(typeof(DyadicOperatorPragmaType)))
                    {
                        DyadicOperatorPragma p;
                        if (DyadicOperatorPragma.HasCondition(t))
                        {
                            p = new DyadicOperatorPragma(t, a, b, bl);
                        }
                        else if (nt[a] >= nt[b])
                        {
                            p = new DyadicOperatorPragma(t, a, b, a);
                        }
                        else
                        {
                            p = new DyadicOperatorPragma(t, a, b, b);
                        }
                        BuiltInList.AppendChild(p);
                        OpManager.Append(p);
                    }
                }
            }
        }

        private IReadOnlyDictionary<ClassSymbol, PrimitivePragmaType> GetBuildInNumberType()
        {
            var ret = new Dictionary<ClassSymbol, PrimitivePragmaType>();
            ret.Add((ClassSymbol)NameResolution("SByte").FindDataType(), PrimitivePragmaType.Integer8);
            ret.Add((ClassSymbol)NameResolution("Int16").FindDataType(), PrimitivePragmaType.Natural16);
            ret.Add((ClassSymbol)NameResolution("Int32").FindDataType(), PrimitivePragmaType.Integer32);
            ret.Add((ClassSymbol)NameResolution("Int64").FindDataType(), PrimitivePragmaType.Integer64);
            ret.Add((ClassSymbol)NameResolution("Byte").FindDataType(), PrimitivePragmaType.Natural8);
            ret.Add((ClassSymbol)NameResolution("UInt16").FindDataType(), PrimitivePragmaType.Natural16);
            ret.Add((ClassSymbol)NameResolution("UInt32").FindDataType(), PrimitivePragmaType.Natural32);
            ret.Add((ClassSymbol)NameResolution("UInt64").FindDataType(), PrimitivePragmaType.Natural64);
            ret.Add((ClassSymbol)NameResolution("Single").FindDataType(), PrimitivePragmaType.Binary32);
            ret.Add((ClassSymbol)NameResolution("Double").FindDataType(), PrimitivePragmaType.Binary64);
            return ret;
        }
    }
}
