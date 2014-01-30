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

        public Translator(FullName fullname, Translator parent)
        {
            Name = fullname.Name;
            Parent = parent;
            Child = new List<Translator>();
            RegisterTranslator(fullname, this);
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

        public virtual Translator FindTranslator(FullName fullName)
        {
            return Parent.FindTranslator(fullName);
        }

        public virtual void RegisterTranslator(FullName fullName, Translator trans)
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

        public virtual Translator GenelateModule(FullName gen)
        {
            return Parent.GenelateModule(gen);
        }

        public virtual Translator GenelateType(FullName gen)
        {
            throw new NotSupportedException();
        }

        public virtual Translator GenelateRoutine(FullName gen)
        {
            throw new NotSupportedException();
        }

        public virtual Translator GenelateVariant(FullName gen, FullName type)
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

        public virtual void GenelateOperate(FullName type, TokenType operation)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateLoad(FullName type)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateStore(FullName type)
        {
            throw new NotSupportedException();
        }
    }
}
