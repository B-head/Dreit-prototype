using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class TypeManager
    {
        private Root Root;
        private List<TypeQualifySymbol> TypeQualifyList;
        private List<TemplateInstanceSymbol> TemplateInstanceList;

        public TypeManager(Root root)
        {
            Root = root;
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
            baseType.AppendChild(ret); //todo foreachで走査している時に要素を追加できない問題に対処する。
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
            TemplateInstanceList.Add(ret);
            return ret;
        }
    }
}
