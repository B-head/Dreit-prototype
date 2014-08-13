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
        private List<TemplateInstanceSymbol> TemplateInstanceList;

        public TemplateInstanceManager()
        {
            TemplateInstanceList = new List<TemplateInstanceSymbol>();
        }
        
        //todo インスタンスの再利用をする。
        public TemplateInstanceSymbol IssueTemplateInstance(Scope baseType, params Scope[] parameter)
        {
            var ret = new TemplateInstanceSymbol(baseType, parameter);
            AppendChild(ret);
            TemplateInstanceList.Add(ret);
            return ret;
        }

        public TemplateInstanceSymbol IssueTemplateInstance(OverLoad template, IReadOnlyList<Scope> parameter)
        {
            var ret = TemplateInstanceList.FirstOrDefault(v => v.Template == template && v.Parameter.SequenceEqual(parameter));
            if (ret != null)
            {
                return ret;
            }
            ret = new TemplateInstanceSymbol(template, parameter);
            AppendChild(ret);
            TemplateInstanceList.Add(ret);
            return ret;
        }
    }
}
