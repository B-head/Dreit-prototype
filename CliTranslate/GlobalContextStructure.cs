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
    public class GlobalContextStructure : ContainerStructure
    {
        public string Name { get; private set; }
        public TypeStructure GlobalField { get; private set; }
        public MethodStructure GlobalContext { get; private set; }

        public GlobalContextStructure(string name)
        {
            Name = name;
            var tattr = TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.SpecialName;
            var gnr = new List<GenericParameterStructure>();
            var imp = new List<TypeStructure>();
            GlobalField =  new PureTypeStructure(Name + ".@@Global", tattr, gnr, null, imp);
            AppendChild(GlobalField);
            var mattr = MethodAttributes.PrivateScope | MethodAttributes.SpecialName | MethodAttributes.Static;
            var arg = new List<ParameterStructure>();
            GlobalContext = new MethodStructure("@@global", mattr, gnr, arg, null);
            GlobalField.AppendChild(GlobalContext);
        }

        internal override CodeGenerator GainGenerator()
        {
            return GlobalContext.GainGenerator();
        }

        internal override TypeBuilder CreateType(string name, System.Reflection.TypeAttributes attr)
        {
            var cont = CurrentContainer;
            return cont.CreateType(name, attr);
        }

        internal override MethodBuilder CreateMethod(string name, MethodAttributes attr)
        {
            return GlobalField.CreateMethod(name, attr);
        }

        internal override FieldBuilder CreateField(string name, Type dt, FieldAttributes attr)
        {
            return GlobalField.CreateField(name, dt, attr);
        }
    }
}
