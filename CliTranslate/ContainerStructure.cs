using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public abstract class ContainerStructure : BuilderStructure
    {
        internal virtual bool IsDataTypeContext
        {
            get { return false; }
        }

        internal virtual CodeGenerator GainGenerator()
        {
            throw new NotSupportedException();
        }

        internal virtual TypeBuilder CreateType(string name, TypeAttributes attr)
        {
            throw new NotSupportedException();
        }

        internal virtual MethodBuilder CreateMethod(string name, MethodAttributes attr)
        {
            throw new NotSupportedException();
        }

        internal virtual ConstructorBuilder CreateConstructor(MethodAttributes attr, Type[] pt)
        {
            throw new NotSupportedException();
        }

        internal virtual FieldBuilder CreateField(string name, Type dt, FieldAttributes attr)
        {
            throw new NotSupportedException();
        }
    }
}
