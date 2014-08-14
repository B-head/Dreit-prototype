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
        public IReadOnlyList<Scope> Parameters { get; private set; }

        public TemplateInstanceSymbol(OverLoad template, IReadOnlyList<Scope> parameter)
        {
            Template = template;
            Parameters = parameter;
        }

        public override bool IsDataType
        {
            get { return true; }
        }

        public override OverLoad OverLoad
        {
            get { return Template; }
        }

        public Scope BaseType
        {
            get { return Template.FindDataType(); }
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
                n = Parameters[0].NameResolution(name);
            }
            ReferenceCache.Add(name, n);
            return n;
        }

        internal bool ContainClass(ClassSymbol cls)
        {
            var m = Template.FindDataType() as ModifyTypeSymbol;
            if (m != null && HasInheritModify(m.ModifyType))
            {
                return Parameters[0] == cls;
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
