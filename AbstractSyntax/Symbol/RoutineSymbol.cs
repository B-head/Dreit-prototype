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
        protected IReadOnlyList<Scope> _Attribute;
        protected IReadOnlyList<GenericSymbol> _Generics;
        protected IReadOnlyList<ParameterSymbol> _Arguments;
        protected IReadOnlyList<Scope> _ArgumentTypes;
        protected Scope _CallReturnType;
        public const string ConstructorIdentifier = "new";
        public const string DestructorIdentifier = "free";

        protected RoutineSymbol(RoutineType type, TokenType opType)
        {
            RoutineType = type;
            OperatorType = opType;
            Block = new ProgramContext();
            _Attribute = new List<Scope>();
            _Generics = new List<GenericSymbol>();
            _Arguments = new List<ParameterSymbol>();
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

        public RoutineSymbol(string name, RoutineType type, TokenType opType, IReadOnlyList<Scope> attr, IReadOnlyList<GenericSymbol> gnr, IReadOnlyList<ParameterSymbol> arg, Scope rt)
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

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<GenericSymbol> Generics
        {
            get { return _Generics; }
        }

        public virtual IReadOnlyList<ParameterSymbol> Arguments
        {
            get { return _Arguments; }
        }

        public virtual IReadOnlyList<Scope> ArgumentTypes
        {
            get
            {
                if (_ArgumentTypes != null)
                {
                    return _ArgumentTypes;
                }
                var a = new List<Scope>();
                foreach (var v in Arguments)
                {
                    var temp = v.ReturnType;
                    a.Add(temp);
                }
                _ArgumentTypes = a;
                return _ArgumentTypes;
            }
        }

        public override Scope CallReturnType
        {
            get { return _CallReturnType; }
        }

        public virtual bool IsVirtual //todo オーバーライドされる可能性が無ければnon-virtualにする。
        {
            get { return HasAnyAttribute(Attribute, AttributeType.Virtual); }
        }

        public virtual bool IsAbstract
        {
            get { return HasAnyAttribute(Attribute, AttributeType.Abstract); }
        }

        public virtual bool IsFunction
        {
            get { return RoutineType == RoutineType.Function || RoutineType == RoutineType.FunctionConverter || RoutineType == RoutineType.FunctionOperator; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<Scope> pars, IReadOnlyList<Scope> args)
        {
            yield return TypeMatch.MakeTypeMatch(Root.ConvManager, this, pars, Generics, args, ArgumentTypes);
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
                return i.ZeroArgInitializer; //todo インポートされたコンストラクターも返すようにする。
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
    }
}
