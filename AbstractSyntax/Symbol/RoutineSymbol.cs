using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    public enum RoutineType
    {
        Unknown,
        Routine,
        Function,
        RoutineOperator,
        FunctionOperator,
        RoutineConverter,
        FunctionConverter,
    }

    [Serializable]
    public class RoutineSymbol : Scope
    {
        public RoutineType RoutineType { get; private set; }
        public TokenType OperatorType { get; private set; }
        public ProgramContext Block { get; private set; }
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected IReadOnlyList<GenericSymbol> _Generics;
        protected IReadOnlyList<ParameterSymbol> _Arguments;
        protected TypeSymbol _CallReturnType;
        public const string ConstructorIdentifier = "new";
        public const string DestructorIdentifier = "free";
        public const string AliasCallIdentifier = "call";

        protected RoutineSymbol(RoutineType type, TokenType opType)
        {
            RoutineType = type;
            OperatorType = opType;
            Block = new ProgramContext();
            AppendChild(Block);
        }

        protected RoutineSymbol(TextPosition tp, string name, RoutineType type, TokenType opType, ProgramContext block)
            : base(tp)
        {
            Name = name;
            RoutineType = type;
            OperatorType = opType;
            Block = block;
            AppendChild(Block);
        }

        public RoutineSymbol(string name, RoutineType type, TokenType opType, IReadOnlyList<AttributeSymbol> attr, IReadOnlyList<GenericSymbol> gnr, IReadOnlyList<ParameterSymbol> arg, TypeSymbol rt)
        {
            Name = name;
            RoutineType = type;
            OperatorType = opType;
            Block = new ProgramContext();
            _Attribute = attr;
            _Generics = gnr;
            _Arguments = arg;
            _CallReturnType = rt;
            AppendChild(Block);
        }

        public override IReadOnlyList<AttributeSymbol> Attribute
        {
            get { return _Attribute ?? new List<AttributeSymbol>(); }
        }

        public virtual IReadOnlyList<GenericSymbol> Generics
        {
            get { return _Generics ?? new List<GenericSymbol>();; }
        }

        public virtual IReadOnlyList<ParameterSymbol> Arguments
        {
            get { return _Arguments ?? new List<ParameterSymbol>();; }
        }

        protected override string ElementInfo
        {
            get
            {
                if (Generics.Count == 0)
                {
                    return string.Format("{0}", Name);
                }
                else
                {
                    return string.Format("{0}!({1})", Name, Generics.ToNames());
                }
            }
        }

        public override TypeSymbol ReturnType
        {
            get { return Root.ErrorType; } //todo デリゲート型を返すようにする。
        }

        public override OverLoad OverLoad
        {
            get { return Root.SimplexManager.Issue(this); }
        }

        public virtual TypeSymbol CallReturnType
        {
            get { return _CallReturnType ?? Root.ErrorType; }
        }

        public override bool IsConstant
        {
            get { return true; }
        }

        public virtual bool IsVirtual //todo オーバーライドされる可能性が無ければnon-virtualにする。
        {
            get { return Attribute.HasAnyAttribute(AttributeType.Virtual); }
        }

        public virtual bool IsAbstract
        {
            get { return Attribute.HasAnyAttribute(AttributeType.Abstract); }
        }

        public virtual bool IsFunction
        {
            get { return RoutineType == RoutineType.Function || RoutineType == RoutineType.FunctionConverter || RoutineType == RoutineType.FunctionOperator; }
        }

        internal override IEnumerable<OverLoadMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadMatch.MakeOverLoadMatch(Root, this, Generics, Arguments, inst, pars, args);
        }

        public RoutineSymbol InheritInitializer
        {
            get
            {
                var cls = GetParent<ClassSymbol>();
                if(cls == null)
                {
                    return null;
                }
                if (cls.InheritClass == null)
                {
                    return null;
                }
                var i = cls.InheritClass as ClassSymbol;
                return i.ZeroArgInitializer;
            }
        }

        public bool IsConstructor
        {
            get
            {
                if (!(CurrentScope is ClassSymbol))
                {
                    return false;
                }
                if (Name != ConstructorIdentifier)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsDestructor
        {
            get
            {
                if (!(CurrentScope is ClassSymbol))
                {
                    return false;
                }
                if (Name != DestructorIdentifier)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsAliasCall
        {
            get
            {
                if (!(CurrentScope is ClassSymbol))
                {
                    return false;
                }
                if (Name != AliasCallIdentifier)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
