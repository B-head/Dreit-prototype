using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class TypeManager : Element
    {
        private List<TypeQualifySymbol> TypeQualifyList;
        private List<TemplateInstanceSymbol> TemplateInstanceList;

        public TypeManager()
        {
            TypeQualifyList = new List<TypeQualifySymbol>();
            TemplateInstanceList = new List<TemplateInstanceSymbol>();
        }

        public TypeQualifySymbol IssueTypeQualify(Scope baseType, params AttributeSymbol[] qualify)
        {
            var ret = TypeQualifyList.FirstOrDefault(v => v.BaseType == baseType && v.Qualify.SequenceEqual(qualify));
            if(ret != null)
            {
                return ret;
            }
            ret = new TypeQualifySymbol(baseType, qualify);
            AppendChild(ret);
            TypeQualifyList.Add(ret);
            return ret;
        }

        public TemplateInstanceSymbol IssueTemplateInstance(OverLoadReference template, params Scope[] parameter)
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
