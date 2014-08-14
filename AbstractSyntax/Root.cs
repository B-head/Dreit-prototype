using AbstractSyntax.SpecialSymbol;
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
        private NameSpaceSymbol EmbedList;
        public CompileMessageManager MessageManager { get; private set; }
        public OverLoadSimplexManager SimplexManager { get; private set; }
        public TemplateInstanceManager TemplateInstanceManager { get; private set; }
        internal ConversionManager ConvManager { get; private set; }
        internal OperationManager OpManager { get; private set; }
        internal OverLoadChain UndefinedOverLord { get; private set; }
        public VoidSymbol Void { get; private set; }
        public UnknownSymbol Unknown { get; private set; }
        public ErrorTypeSymbol ErrorType { get; private set; }
        public ErrorRoutineSymbol ErrorRoutine { get; private set; }
        public DefaultSymbol Default { get; private set; }
        public ModifyTypeSymbol Refer { get; private set; }
        public ModifyTypeSymbol Typeof { get; private set; }
        public ModifyTypeSymbol Nullable { get; private set; }
        public ModifyTypeSymbol Pointer { get; private set; }
        public ModifyTypeSymbol EmbedArray { get; private set; }
        public AttributeSymbol Contravariant { get; private set; }
        public AttributeSymbol Covariant { get; private set; }
        public AttributeSymbol ConstructorConstraint { get; private set; }
        public AttributeSymbol ValueConstraint { get; private set; }
        public AttributeSymbol ReferenceConstraint { get; private set; }
        public AttributeSymbol Variadic { get; private set; }
        public AttributeSymbol Optional { get; private set; }
        public AttributeSymbol GlobalScope { get; private set; }
        public AttributeSymbol Abstract { get; private set; }
        public AttributeSymbol Virtual { get; private set; }
        public AttributeSymbol Final { get; private set; }
        public AttributeSymbol Static { get; private set; }
        public AttributeSymbol Public { get; private set; }
        public AttributeSymbol Internal { get; private set; }
        public AttributeSymbol Protected { get; private set; }
        public AttributeSymbol Private { get; private set; }

        public Root()
        {
            Name = "global";
            EmbedList = new NameSpaceSymbol();
            MessageManager = new CompileMessageManager();
            SimplexManager = new OverLoadSimplexManager();
            TemplateInstanceManager = new TemplateInstanceManager(SimplexManager);
            ConvManager = new ConversionManager(this);
            OpManager = new OperationManager(this); 
            UndefinedOverLord = new OverLoadChain(this, null);
            CreateEmbedIdentifier();
            AppendChild(EmbedList);
            AppendChild(TemplateInstanceManager);
        }

        public void SemanticAnalysis()
        {
            CreateEmbedOperator();
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

        private void CreateEmbedIdentifier()
        {
            Void = new VoidSymbol();
            Unknown = new UnknownSymbol();
            ErrorType = new ErrorTypeSymbol();
            ErrorRoutine = new ErrorRoutineSymbol();
            Default = new DefaultSymbol();
            Refer = new ModifyTypeSymbol(ModifyType.Refer);
            Typeof = new ModifyTypeSymbol(ModifyType.Typeof);
            Nullable = new ModifyTypeSymbol(ModifyType.Nullable);
            Pointer = new ModifyTypeSymbol(ModifyType.Pointer);
            EmbedArray = new ModifyTypeSymbol(ModifyType.EmbedArray);
            Contravariant = new AttributeSymbol(AttributeType.Contravariant);
            Covariant = new AttributeSymbol(AttributeType.Covariant);
            ConstructorConstraint = new AttributeSymbol(AttributeType.ConstructorConstraint);
            ValueConstraint = new AttributeSymbol(AttributeType.ValueConstraint);
            ReferenceConstraint = new AttributeSymbol(AttributeType.ReferenceConstraint);
            Variadic = new AttributeSymbol(AttributeType.Variadic, "variadic");
            Optional = new AttributeSymbol(AttributeType.Optional);
            GlobalScope = new AttributeSymbol(AttributeType.GlobalScope);
            Abstract = new AttributeSymbol(AttributeType.Abstract);
            Virtual = new AttributeSymbol(AttributeType.Virtual);
            Final = new AttributeSymbol(AttributeType.Final, "final");
            Static = new AttributeSymbol(AttributeType.Static, "static");
            Public = new AttributeSymbol(AttributeType.Public, "public");
            Internal = new AttributeSymbol(AttributeType.Internal, "internal");
            Protected = new AttributeSymbol(AttributeType.Protected, "protected");
            Private = new AttributeSymbol(AttributeType.Private, "private");
            EmbedList.AppendChild(Void);
            EmbedList.AppendChild(Unknown);
            EmbedList.AppendChild(ErrorType);
            EmbedList.AppendChild(ErrorRoutine);
            EmbedList.AppendChild(Default);
            EmbedList.AppendChild(Refer);
            EmbedList.AppendChild(Typeof);
            EmbedList.AppendChild(Nullable);
            EmbedList.AppendChild(Pointer);
            EmbedList.AppendChild(EmbedArray);
            EmbedList.AppendChild(Variadic);
            EmbedList.AppendChild(Optional);
            EmbedList.AppendChild(GlobalScope);
            EmbedList.AppendChild(Virtual);
            EmbedList.AppendChild(Final);
            EmbedList.AppendChild(Static);
            EmbedList.AppendChild(Public);
            EmbedList.AppendChild(Internal);
            EmbedList.AppendChild(Protected);
            EmbedList.AppendChild(Private);
            EmbedList.AppendChild(new BooleanSymbol(false));
            EmbedList.AppendChild(new BooleanSymbol(true));
        }

        private void CreateEmbedOperator()
        {
            var nt = GetBuildInNumberType();
            CreateEmbedCast(nt);
            CreateBuiltInMonadicOperator(nt);
            CreateBuiltInDyadicOperator(nt);
        }

        private void CreateEmbedCast(IReadOnlyDictionary<ClassSymbol, PrimitiveType> nt)
        {
            foreach(var a in nt.Keys)
            {
                foreach(var b in nt.Keys)
                {
                    if(a == b)
                    {
                        continue;
                    }
                    var p = new CastSymbol(nt[b], a, b);
                    EmbedList.AppendChild(p);
                    ConvManager.Append(p);
                }
            }
        }

        private void CreateBuiltInMonadicOperator(IReadOnlyDictionary<ClassSymbol, PrimitiveType> nt)
        {
            var bl = (ClassSymbol)NameResolution("Boolean").FindDataType();
            var nbp = new MonadicOperatorSymbol(TokenType.Not, bl, bl);
            EmbedList.AppendChild(nbp);
            OpManager.Append(nbp);
            foreach (var a in nt.Keys)
            {
                foreach (TokenType t in MonadicOperatorSymbol.EnumOperator())
                {
                    MonadicOperatorSymbol p;
                    if (MonadicOperatorSymbol.HasCondition(t))
                    {
                        p = new MonadicOperatorSymbol(t, a, bl);
                    }
                    else
                    {
                        p = new MonadicOperatorSymbol(t, a, a);
                    }
                    EmbedList.AppendChild(p);
                    OpManager.Append(p);
                }
            }
        }

        private void CreateBuiltInDyadicOperator(IReadOnlyDictionary<ClassSymbol, PrimitiveType> nt)
        {
            var bl = (ClassSymbol)NameResolution("Boolean").FindDataType();
            foreach (var a in nt.Keys)
            {
                foreach (var b in nt.Keys)
                {
                    foreach (TokenType t in DyadicOperatorSymbol.EnumOperator())
                    {
                        DyadicOperatorSymbol p;
                        if (DyadicOperatorSymbol.HasCondition(t))
                        {
                            p = new DyadicOperatorSymbol(t, a, b, bl);
                        }
                        else if (nt[a] >= nt[b])
                        {
                            p = new DyadicOperatorSymbol(t, a, b, a);
                        }
                        else
                        {
                            p = new DyadicOperatorSymbol(t, a, b, b);
                        }
                        EmbedList.AppendChild(p);
                        OpManager.Append(p);
                    }
                }
            }
        }

        private IReadOnlyDictionary<ClassSymbol, PrimitiveType> GetBuildInNumberType()
        {
            var ret = new Dictionary<ClassSymbol, PrimitiveType>();
            ret.Add((ClassSymbol)NameResolution("SByte").FindDataType(), PrimitiveType.Integer8);
            ret.Add((ClassSymbol)NameResolution("Int16").FindDataType(), PrimitiveType.Natural16);
            ret.Add((ClassSymbol)NameResolution("Int32").FindDataType(), PrimitiveType.Integer32);
            ret.Add((ClassSymbol)NameResolution("Int64").FindDataType(), PrimitiveType.Integer64);
            ret.Add((ClassSymbol)NameResolution("Byte").FindDataType(), PrimitiveType.Natural8);
            ret.Add((ClassSymbol)NameResolution("UInt16").FindDataType(), PrimitiveType.Natural16);
            ret.Add((ClassSymbol)NameResolution("UInt32").FindDataType(), PrimitiveType.Natural32);
            ret.Add((ClassSymbol)NameResolution("UInt64").FindDataType(), PrimitiveType.Natural64);
            ret.Add((ClassSymbol)NameResolution("Single").FindDataType(), PrimitiveType.Binary32);
            ret.Add((ClassSymbol)NameResolution("Double").FindDataType(), PrimitiveType.Binary64);
            return ret;
        }
    }
}
