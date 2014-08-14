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
    public class TemplateInstanceManager : Element
    {
        private Dictionary<OverLoad, List<TemplateInstanceSymbol>> TemplateDictonary;
        private OverLoadSimplexManager SimplexManager;

        public TemplateInstanceManager(OverLoadSimplexManager simplexManager)
        {
            TemplateDictonary = new Dictionary<AbstractSyntax.OverLoad, List<TemplateInstanceSymbol>>();
            SimplexManager = simplexManager;
        }
        
        public TemplateInstanceSymbol Issue(Scope baseType, params Scope[] parameter)
        {
            var ol = SimplexManager.Issue(baseType);
            return Issue(ol, parameter);
        }

        public TemplateInstanceSymbol Issue(Scope baseType, IReadOnlyList<Scope> parameter)
        {
            var ol = SimplexManager.Issue(baseType);
            return Issue(ol, parameter);
        }

        public TemplateInstanceSymbol Issue(OverLoad template, IReadOnlyList<Scope> parameter)
        {
            var ret = FindInstance(template, parameter);
            if (ret != null)
            {
                return ret;
            }
            ret = new TemplateInstanceSymbol(template, parameter);
            AppendInstance(ret);
            return ret;
        }

        private void AppendInstance(TemplateInstanceSymbol instance)
        {
            if(!TemplateDictonary.ContainsKey(instance.OverLoad))
            {
                TemplateDictonary.Add(instance.OverLoad, new List<TemplateInstanceSymbol>());
            }
            TemplateDictonary[instance.OverLoad].Add(instance);
            AppendChild(instance);
        }

        private TemplateInstanceSymbol FindInstance(OverLoad template, IReadOnlyList<Scope> parameter)
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
