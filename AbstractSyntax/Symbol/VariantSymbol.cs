using AbstractSyntax.Expression;
using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    public enum VariantType
    {
        Unknown,
        Var,
        Let,
        Const,
    }

    [Serializable]
    public class VariantSymbol : Scope
    {
        public VariantType VariantType { get; private set; }
        public Element DefaultValue { get; private set; }
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected TypeSymbol _DataType;
        private bool IsInitialize;

        public VariantSymbol()
        {
        }

        protected VariantSymbol(VariantType type, Element def = null)
        {
            VariantType = type;
            DefaultValue = def;
            AppendChild(DefaultValue);
            IsInitialize = true;
        }

        protected VariantSymbol(TextPosition tp, VariantType type, Element def = null)
            : base(tp)
        {
            VariantType = type;
            DefaultValue = def;
            AppendChild(DefaultValue);
            IsInitialize = true;
        }

        public void Initialize(string name, VariantType type, IReadOnlyList<AttributeSymbol> attr, TypeSymbol dt, Element def = null)
        {
            if(IsInitialize)
            {
                throw new InvalidOperationException();
            }
            IsInitialize = true;
            Name = name;
            VariantType = type;
            DefaultValue = def;
            AppendChild(DefaultValue);
            _Attribute = attr;
            _DataType = dt;
        }

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get { return _Attribute ?? new List<AttributeSymbol>(); }
        }

        public override TypeSymbol ReturnType
        {
            get { return DataType; }
        }

        public virtual TypeSymbol DataType
        {
            get { return _DataType ?? Root.ErrorType; }
        }

        public override bool IsConstant
        {
            get { return true; }
        }

        public bool IsClassField
        {
            get { return CurrentScope is ClassSymbol; }
        }

        public bool IsEnumField
        {
            get { return CurrentScope is EnumSymbol; }
        }

        public bool IsLocal
        {
            get { return CurrentScope is RoutineSymbol; }
        }

        public bool IsGlobal
        {
            get { return CurrentScope is NameSpaceSymbol; }
        }

        public bool IsLoopVariant
        {
            get { return CurrentScope is LoopStatement || CurrentScope is ForStatement; }
        }

        public bool IsDefinedConstantValue
        {
            get { return Parent is CallExpression && CurrentScope is ClassSymbol; }
        }

        public bool IsImmtable
        {
            get { return VariantType == VariantType.Let || VariantType == VariantType.Const || VariantType == VariantType.Unknown; }
        }

        public object GenerateConstantValue()
        {
            var v = DefaultValue as ValueSymbol;
            if(v != null)
            {
                return v.Value;
            }
            var caller = Parent as CallExpression;
            if (caller == null)
            {
                return null;
            }
            return caller.GenelateConstantValue();
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (ReturnType is UnknownSymbol)
            {
                cmm.CompileError("require-type", this);
            }
        }
    }
}
