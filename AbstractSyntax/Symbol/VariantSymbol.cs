using AbstractSyntax.Expression;
using AbstractSyntax.SpecialSymbol;
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
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected TypeSymbol _DataType;

        protected VariantSymbol(VariantType type)
        {
            VariantType = type;
        }

        protected VariantSymbol(TextPosition tp, VariantType type)
            : base(tp)
        {
            VariantType = type;
        }

        public VariantSymbol(string name, VariantType type, IReadOnlyList<AttributeSymbol> attr, TypeSymbol dt)
        {
            Name = name;
            VariantType = type;
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

        public bool IsField
        {
            get { return CurrentScope is ClassSymbol; }
        }

        public bool IsLocal
        {
            get { return CurrentScope is RoutineSymbol; }
        }

        public bool IsGlobal
        {
            get { return CurrentScope is NameSpaceSymbol; }
        }

        public bool IsDefinedConstantValue
        {
            get { return Parent is CallExpression && CurrentScope is ClassSymbol; }
        }

        public bool IsImmtable
        {
            get { return VariantType == VariantType.Let || VariantType == VariantType.Const; }
        }

        public object GenerateConstantValue()
        {
            var caller = Parent as CallExpression;
            if (caller == null)
            {
                return null;
            }
            return caller.GenelateConstantValue();
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if (ReturnType is ErrorTypeSymbol)
            {
                cmm.CompileError("require-type", this);
            }
        }
    }
}
