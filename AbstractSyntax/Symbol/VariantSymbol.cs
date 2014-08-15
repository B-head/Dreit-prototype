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
        protected IReadOnlyList<Scope> _Attribute;
        protected Scope _DataType;

        protected VariantSymbol(VariantType type)
        {
            VariantType = type;
        }

        protected VariantSymbol(TextPosition tp, VariantType type)
            : base(tp)
        {
            VariantType = type;
        }

        public VariantSymbol(string name, VariantType type, IReadOnlyList<Scope> attr, Scope dt)
        {
            Name = name;
            VariantType = type;
            _Attribute = attr;
            _DataType = dt;
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute ?? new List<AttributeSymbol>(); }
        }

        public override Scope ReturnType
        {
            get { return CallReturnType; }
        }

        public override Scope CallReturnType
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

        internal override IEnumerable<OverLoadMatch> GetTypeMatch(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            var type = CallReturnType as ClassSymbol;
            if(type == null)
            {
                yield return OverLoadMatch.MakeUnknown(Root.ErrorRoutine);
                yield break;
            }
            foreach (var v in type.GetInstanceTypeMatch(pars, args))
            {
                yield return v;
            }
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
