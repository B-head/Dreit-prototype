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

        public AttributeSymbol(string name, ClassType type, ProgramContext block, IReadOnlyList<AttributeSymbol> attr, IReadOnlyList<GenericSymbol> gnr, IReadOnlyList<TypeSymbol> inherit, AttributeTargets validon, bool isMulti, bool isInherit)
            :base(name, type, block, attr, gnr, inherit)
        {
            AttributeType = AttributeType.Custom;
            ValidOn = validon;
            IsAllowMultiple = isMulti;
            IsInheritable = isInherit;
        }
    }

    //todo 単純なフラグ管理で属性を扱うようにリファクタリングする。
    public enum AttributeType
    {
        Unknown,
        Custom,
        Contravariant,
        Covariant,
        ConstructorConstraint,
        ValueConstraint,
        ReferenceConstraint,
        Variadic,
        Optional,
        GlobalScope,
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
