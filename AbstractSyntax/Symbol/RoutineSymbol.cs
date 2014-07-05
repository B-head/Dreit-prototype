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
        protected List<IScope> _Attribute;
        protected List<IDataType> _ArgumentType;
        protected IDataType _ReturnType;

        protected RoutineSymbol()
        {

        }

        protected RoutineSymbol(TextPosition tp, string name, TokenType op, bool isFunc)
            : base(tp)
        {
            Name = name;
            Operator = op;
            IsFunction = isFunc;
        }

        public override IReadOnlyList<IScope> Attribute
        {
            get { return _Attribute; }
        }

        public virtual IReadOnlyList<IDataType> ArgumentType
        {
            get { return _ArgumentType; }
        }

        public override IDataType CallReturnType
        {
            get { return _ReturnType; }
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

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, ArgumentType);
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
