using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Dlight.CilTranslate
{
    abstract class CilTranslator : Translator
    {
        public string Name { get; private set; }
        public CilTranslator Parent { get; private set; }
        protected List<CilTranslator> Child { get; private set; }

        public CilTranslator(string name, CilTranslator parent = null)
        {
            Name = name;
            Parent = parent;
            Child = new List<CilTranslator>();
        }

        public CilTranslator(Scope<Element> scope, CilTranslator parent)
        {
            Name = scope.Name;
            Parent = parent;
            Child = new List<CilTranslator>();
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

        public virtual CilTranslator FindTranslator(string fullName)
        {
            return Parent.FindTranslator(fullName);
        }

        public virtual void RegisterTranslator(string fullName, CilTranslator trans)
        {
            Parent.RegisterTranslator(fullName, trans);
        }

        public virtual void Save()
        {
            foreach (CilTranslator v in Child)
            {
                v.Save();
            }
        }

        public virtual Translator CreateModule(Scope<Element> scope)
        {
            return Parent.CreateModule(scope);
        }

        public virtual Translator CreateVariable(Scope<Element> scope, string fullName)
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

        public virtual void GenelateNumber(int value)
        {
            throw new NotSupportedException();
        }

        public virtual void GenelateBinomial(string fullName, TokenType operation)
        {
            throw new NotSupportedException();
        }
    }
}
