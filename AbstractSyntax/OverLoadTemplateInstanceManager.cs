using AbstractSyntax.SpecialSymbol;
using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class OverLoadTemplateInstanceManager
    {
        private Dictionary<OverLoad, List<OverLoadTemplateInstance>> TemplateDictonary;

        public OverLoadTemplateInstanceManager()
        {
            TemplateDictonary = new Dictionary<AbstractSyntax.OverLoad, List<OverLoadTemplateInstance>>();
        }

        public OverLoadTemplateInstance Issue(OverLoad template, IReadOnlyList<TypeSymbol> parameter)
        {
            var ret = FindInstance(template, parameter);
            if (ret != null)
            {
                return ret;
            }
            ret = new OverLoadTemplateInstance(template, parameter);
            AppendInstance(ret);
            return ret;
        }

        private void AppendInstance(OverLoadTemplateInstance instance)
        {
            if(!TemplateDictonary.ContainsKey(instance.OverLoad))
            {
                TemplateDictonary.Add(instance.OverLoad, new List<OverLoadTemplateInstance>());
            }
            TemplateDictonary[instance.OverLoad].Add(instance);
        }

        private OverLoadTemplateInstance FindInstance(OverLoad template, IReadOnlyList<Scope> parameter)
        {
            if (!TemplateDictonary.ContainsKey(template))
            {
                return null;
            }
            var list = TemplateDictonary[template];
            var ret = list.FirstOrDefault(v => v.Parameters.SequenceEqual(parameter));
            return ret;
        }
    }
}
