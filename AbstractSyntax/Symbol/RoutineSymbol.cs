using AbstractSyntax.Declaration;
using AbstractSyntax.Expression;
using AbstractSyntax.SpecialSymbol;
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
        public bool IsDefaultThisReturn { get; protected set; }
        protected IReadOnlyList<AttributeSymbol> _Attribute;
        protected IReadOnlyList<GenericSymbol> _Generics;
        protected IReadOnlyList<ArgumentSymbol> _Arguments;
        protected TypeSymbol _CallReturnType;
        private IReadOnlyList<GenericSymbol> _TacitGeneric;
        private bool IsInitialize;
        public const string ConstructorIdentifier = "new";
        public const string DestructorIdentifier = "free";
        public const string AliasCallIdentifier = "call";

        public RoutineSymbol()
        {
        }

        protected RoutineSymbol(RoutineType type, TokenType opType)
        {
            RoutineType = type;
            OperatorType = opType;
            Block = new ProgramContext();
            AppendChild(Block);
            IsInitialize = true;
        }

        protected RoutineSymbol(TextPosition tp, string name, RoutineType type, TokenType opType, ProgramContext block)
            : base(tp)
        {
            Name = name;
            RoutineType = type;
            OperatorType = opType;
            Block = block;
            AppendChild(Block);
            IsInitialize = true;
        }

        public void Initialize(string name, RoutineType type, TokenType opType, IReadOnlyList<AttributeSymbol> attr, IReadOnlyList<GenericSymbol> gnr, IReadOnlyList<ArgumentSymbol> arg, TypeSymbol rt)
        {
            if (IsInitialize)
            {
                throw new InvalidOperationException();
            }
            IsInitialize = true;
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

        public virtual IReadOnlyList<ArgumentSymbol> Arguments
        {
            get { return _Arguments ?? new List<ArgumentSymbol>();; }
        }

        public IReadOnlyList<GenericSymbol> TacitGeneric
        {
            get
            {
                if(_TacitGeneric != null)
                {
                    return _TacitGeneric;
                }
                var list = new List<GenericSymbol>();
                CurrentScope.BuildTacitGeneric(list);
                _TacitGeneric = list;
                return _TacitGeneric;
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

        public bool IsVirtual //todo オーバーライドされる可能性が無ければnon-virtualにする。
        {
            get
            {
                if(Attribute.HasAnyAttribute(AttributeType.Virtual))
                {
                    return true;
                }
                var cls = GetParent<ClassSymbol>();
                if (cls == null)
                {
                    return false;
                }
                return IsInstanceMember;
            }
        }

        public bool IsAbstract
        {
            get { return Attribute.HasAnyAttribute(AttributeType.Abstract); }
        }

        public bool IsFunction
        {
            get { return RoutineType == RoutineType.Function || RoutineType == RoutineType.FunctionConverter || RoutineType == RoutineType.FunctionOperator; }
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

        internal override void Prepare()
        {
            if(IsOperator)
            {
                Root.OpManager.Append(this);
            }
            else if(IsConverter)
            {
                Root.ConvManager.Append(this);
            }
        }

        internal override void BuildTacitGeneric(List<GenericSymbol> list)
        {
            if (CurrentScope != null)
            {
                CurrentScope.BuildTacitGeneric(list);
            }
            list.AddRange(Generics);
        }

        internal override IEnumerable<OverLoadCallMatch> GetTypeMatch(IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> pars, IReadOnlyList<TypeSymbol> args)
        {
            yield return OverLoadCallMatch.MakeMatch(Root, this, Generics, Arguments, inst, pars, args);
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

        public override bool IsInstanceMember
        {
            get { return base.IsInstanceMember && !IsConstructor; }
        }

        public override bool IsStaticMember
        {
            get { return base.IsStaticMember && !IsConstructor; }
        }

        public bool IsOperator
        {
            get { return RoutineType == RoutineType.FunctionOperator || RoutineType == RoutineType.RoutineOperator; }
        }

        public bool IsConverter
        {
            get { return RoutineType == RoutineType.FunctionConverter || RoutineType == RoutineType.RoutineConverter; }
        }

        public virtual bool IsConstructor
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

        public virtual bool IsDestructor
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

        public virtual bool IsAliasCall
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

        public static bool HasLoadStoreCall(RoutineSymbol routine)
        {
            var rti = routine as RoutineTemplateInstance;
            if(rti != null)
            {
                return HasLoadStoreCall(rti.Routine);
            }
            return routine is PropertySymbol;
        }
    }
}
