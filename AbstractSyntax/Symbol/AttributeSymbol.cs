using AbstractSyntax.Expression;
using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class AttributeSymbol : ClassSymbol
    {
        public AttributeType AttributeType { get; private set; }
        public AttributeTargets ValidOn { get; private set; }
        public bool IsAllowMultiple { get; private set; }
        public bool IsInheritable { get; private set; }

        public AttributeSymbol(AttributeType type, string name = null)
        {
            Name = name;
            AttributeType = type;
            ValidOn = AttributeTargets.All;
        }

        public AttributeSymbol(string name, ClassType type, ProgramContext block, IReadOnlyList<Scope> attr, IReadOnlyList<GenericSymbol> gnr, IReadOnlyList<Scope> inherit, AttributeTargets validon, bool isMulti, bool isInherit)
            :base(name, type, block, attr, gnr, inherit)
        {
            AttributeType = AttributeType.Custom;
            ValidOn = validon;
            IsAllowMultiple = isMulti;
            IsInheritable = isInherit;
        }
    }

    public enum AttributeType
    {
        Unknown,
        Custom,
        Refer,
        Tyoeof,
        Variadic,
        Contravariant,
        Covariant,
        ConstructorConstraint,
        ValueConstraint,
        ReferenceConstraint,
        Optional,
        Abstract,
        Virtual,
        Final,
        Static,
        Public,
        Internal,
        Protected,
        Private,
    }
}
