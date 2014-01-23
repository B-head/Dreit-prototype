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

        public abstract MethodInfo GetContext();
        public abstract Type GetDataType();
        public abstract void GenelateNumber(int value);
        public abstract void GenelateBinomial(Type dataType, SyntaxType operation);

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
    }



}
