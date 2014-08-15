using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class ClassTemplateInstanceManager : Element
    {
        private Dictionary<TypeSymbol, List<ClassTemplateInstance>> TemplateDictonary;

        public ClassTemplateInstanceManager()
        {
            TemplateDictonary = new Dictionary<TypeSymbol, List<ClassTemplateInstance>>();
        }

        public ClassTemplateInstance Issue(TypeSymbol type, IReadOnlyList<TypeSymbol> parameter)
        {
            var ret = FindInstance(type, parameter);
            if (ret != null)
            {
                return ret;
            }
            ret = new ClassTemplateInstance(type, parameter);
            AppendInstance(ret);
            return ret;
        }

        private void AppendInstance(ClassTemplateInstance instance)
        {
            if(!TemplateDictonary.ContainsKey(instance.Type))
            {
                TemplateDictonary.Add(instance.Type, new List<ClassTemplateInstance>());
            }
            TemplateDictonary[instance.Type].Add(instance);
            AppendChild(instance);
        }

        private ClassTemplateInstance FindInstance(TypeSymbol type, IReadOnlyList<Scope> parameter)
        {
            if (!TemplateDictonary.ContainsKey(type))
            {
                return null;
            }
            var list = TemplateDictonary[type];
            var ret = list.FirstOrDefault(v => v.Parameters.SequenceEqual(parameter));
            return ret;
        }
    }
}
