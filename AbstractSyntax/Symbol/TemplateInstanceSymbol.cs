using AbstractSyntax.SpecialSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class TemplateInstanceSymbol : Scope
    {
        public OverLoad Template { get; private set; }
        public IReadOnlyList<Scope> Parameter { get; private set; }

        public TemplateInstanceSymbol(Scope baseType, params Scope[] parameter)
        {
            Template = new OverLoadSimplex(baseType);
            Parameter = parameter;
        }

        public TemplateInstanceSymbol(OverLoad template, IReadOnlyList<Scope> parameter)
        {
            Template = template;
            Parameter = parameter;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        public override OverLoad OverLoad
        {
            get { return Template; }
        }

        internal override OverLoadChain NameResolution(string name)
        {
            if (ReferenceCache.ContainsKey(name))
            {
                return ReferenceCache[name];
            }
            OverLoadChain n = Root.UndefinedOverLord;
            var m = Template.FindDataType() as ModifyTypeSymbol;
            if (m != null && HasInheritModify(m.ModifyType))
            {
                n = Parameter[0].NameResolution(name);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        internal bool ContainClass(ClassSymbol cls)
        {
            var m = Template.FindDataType() as ModifyTypeSymbol;
            if (m != null && HasInheritModify(m.ModifyType))
            {
                return Parameter[0] == cls;
            }
            return false;
        }

        private bool HasInheritModify(ModifyType type)
        {
            return type == ModifyType.Refer ||
                 type == ModifyType.Typeof ||
                 type == ModifyType.Nullable ||
                 type == ModifyType.Pointer;
        }
    }
}
