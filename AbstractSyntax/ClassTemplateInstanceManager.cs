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
        private Dictionary<InstanceKey, ClassTemplateInstance> TemplateDictonary;

        public ClassTemplateInstanceManager()
        {
            TemplateDictonary = new Dictionary<InstanceKey, ClassTemplateInstance>();
        }

        public ClassTemplateInstance Issue(TypeSymbol type, IReadOnlyList<TypeSymbol> parameters, IReadOnlyList<TypeSymbol> tacitParameters)
        {
            var ret = FindInstance(type, parameters, tacitParameters);
            if (ret != null)
            {
                return ret;
            }
            ret = new ClassTemplateInstance(type, parameters, tacitParameters);
            AppendInstance(ret);
            return ret;
        }

        private void AppendInstance(ClassTemplateInstance instance)
        {
            var key = new InstanceKey { Type = instance.Type, Parameters = instance.Parameters, TacitParameters = instance.TacitParameters };
            TemplateDictonary.Add(key, instance);
            AppendChild(instance);
        }

        private ClassTemplateInstance FindInstance(TypeSymbol type, IReadOnlyList<TypeSymbol> parameters, IReadOnlyList<TypeSymbol> tacitParameters)
        {
            var key = new InstanceKey { Type = type, Parameters = parameters, TacitParameters = tacitParameters };
            if (!TemplateDictonary.ContainsKey(key))
            {
                return null;
            }
            return TemplateDictonary[key];
        }

        private struct InstanceKey : IEquatable<InstanceKey>
        {
            public TypeSymbol Type { get; set; }
            public IReadOnlyList<TypeSymbol> Parameters { get; set; }
            public IReadOnlyList<TypeSymbol> TacitParameters { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as InstanceKey?;
                if (!other.HasValue)
                {
                    return false;
                }
                return Equals(other.Value);
            }

            public bool Equals(InstanceKey other)
            {
                return Type.Equals(other.Type) &&
                    Parameters.SequenceEqual(other.Parameters) &&
                    TacitParameters.SequenceEqual(other.TacitParameters);
            }

            public override int GetHashCode()
            {
                var hash = Type.GetHashCode();
                foreach (var v in Parameters)
                {
                    hash ^= v.GetHashCode();
                }
                foreach (var v in TacitParameters)
                {

                }
                return base.GetHashCode();
            }
        }
    }
}
