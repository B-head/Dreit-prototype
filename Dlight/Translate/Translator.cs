using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.Translate
{
    abstract class Translator
    {
        public string Name { get; private set; }
        public Translator Parent { get; private set; }
        protected List<Translator> Child { get; private set; }

        public Translator(string name, Translator parent = null)
        {
            Name = name;
            Parent = parent;
            Child = new List<Translator>();
        }

        public Translator(Scope scope, Translator parent)
        {
            Name = scope.Name;
            Parent = parent;
            Child = new List<Translator>();
            RegisterTranslator(scope.GetFullName(), this);
        }

        public virtual MethodInfo GetContext()
        {
            throw new NotSupportedException();
        }

        public virtual LocalBuilder GetLocal()
        {
            throw new NotSupportedException();
        }

        public virtual Type GetDataType()
        {
            throw new NotSupportedException();
        }

        public virtual Translator FindTranslator(string fullName)
        {
            return Parent.FindTranslator(fullName);
        }

        public virtual void RegisterTranslator(string fullName, Translator trans)
        {
            Parent.RegisterTranslator(fullName, trans);
        }

        public virtual void Save()
        {
            foreach (Translator v in Child)
            {
                v.Save();
            }
        }

        public virtual Translator GenelateModule(Scope scope)
        {
            return Parent.GenelateModule(scope);
        }

        public virtual Translator GenelateType(Scope scope)
        {
            throw new NotSupportedException();
        }

        public virtual Translator GenelateRoutine(Scope scope)
        {
            throw new NotSupportedException();
        }

        public virtual Translator GenelateVariant(Scope scope, string fullName)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateConstant(int value)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateConstant(double value)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateCalculate()
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateOperate(string fullName, TokenType operation)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateLoad(string fullName)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateStore(string fullName)
        {
            throw new NotSupportedException();
        }
    }
}
