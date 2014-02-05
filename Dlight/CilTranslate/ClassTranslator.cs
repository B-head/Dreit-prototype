using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight.CilTranslate
{
    class ClassTranslator : ContextTranslator
    {
        public Type TypeInfo { get; private set; }

        public ClassTranslator(string name, Translator parent, Type type = null)
            : base(name, parent)
        {
            TypeInfo = type;
            if (type != null)
            {
                foreach(var f in type.GetFields())
                {
                    new VariantTranslator(f.Name, this, f);
                }
                foreach(var m in type.GetMethods())
                {
                    new RoutineTranslator(m.Name, this, m);
                }
                foreach(var t in type.GetNestedTypes())
                {
                    if(t.IsEnum)
                    {
                        new EnumTranslator(t.Name, this, t);
                    }
                    else
                    {
                        new ClassTranslator(t.Name, this, t);
                    }
                }
            }
        }

        public override Translator CreateGeneric(string name)
        {
            return new GenericTranslator(name, this);
        }
    }
}
