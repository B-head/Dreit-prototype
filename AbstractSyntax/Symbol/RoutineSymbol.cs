using AbstractSyntax.Daclate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class RoutineSymbol : Scope
    {
        public TokenType Operator { get; private set; }
        public bool IsFunction { get; private set; }
        protected IReadOnlyList<Scope> _Attribute;
        protected IReadOnlyList<GenericSymbol> _Generics;
        protected IReadOnlyList<Scope> _ArgumentTypes;
        protected Scope _CallReturnType;

        protected RoutineSymbol()
        {
            _Attribute = new List<Scope>();
            _Generics = new List<GenericSymbol>();
            _ArgumentTypes = new List<Scope>();
        }

        protected RoutineSymbol(TextPosition tp, string name, TokenType op, bool isFunc)
            : base(tp)
        {
            Name = name;
            Operator = op;
            IsFunction = isFunc;
        }

        public override IReadOnlyList<Scope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<GenericSymbol> Generics
        {
            get { return _Generics; }
        }

        public virtual IReadOnlyList<Scope> ArgumentTypes
        {
            get { return _ArgumentTypes; }
        }

        public override Scope CallReturnType
        {
            get { return _CallReturnType; }
        }

        public bool IsVirtual //todo オーバーライドされる可能性が無ければnon-virtualにする。
        {
            get
            { 
                var cls = GetParent<ClassSymbol>();
                if(cls == null)
                {
                    return false;
                }
                return IsInstanceMember && !cls.IsPrimitive; 
            } 
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
                if (cls.InheritClass == null)
                {
                    return null;
                }
                return cls.InheritClass.DefaultInitializer;
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
                if (Name != "new")
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
                if (Name != "free")
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsConvertor
        {
            get
            {
                if (!(CurrentScope is ClassSymbol))
                {
                    return false;
                }
                if (Name == "from" || Name == "to")
                {
                    return true;
                }
                return false;
            }
        }
    }
}
