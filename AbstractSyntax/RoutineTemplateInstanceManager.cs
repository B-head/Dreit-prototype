using AbstractSyntax.Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    [Serializable]
    public class RoutineTemplateInstanceManager : Element
    {
        private Dictionary<InstanceKey, RoutineTemplateInstance> TemplateDictonary;

        public RoutineTemplateInstanceManager()
        {
            TemplateDictonary = new Dictionary<InstanceKey, RoutineTemplateInstance>();
        }

        public RoutineTemplateInstance Issue(RoutineSymbol routine, IReadOnlyList<TypeSymbol> parameters, IReadOnlyList<TypeSymbol> tacitParameters)
        {
            var ret = FindInstance(routine, parameters, tacitParameters);
            if (ret != null)
            {
                return ret;
            }
            ret = new RoutineTemplateInstance(routine, parameters, tacitParameters);
            AppendInstance(ret);
            return ret;
        }

        private void AppendInstance(RoutineTemplateInstance instance)
        {
            var key = new InstanceKey { Routine = instance.Routine, Parameters = instance.Parameters, TacitParameters = instance.TacitParameters };
            TemplateDictonary.Add(key, instance);
            AppendChild(instance);
        }

        private RoutineTemplateInstance FindInstance(RoutineSymbol routine, IReadOnlyList<TypeSymbol> parameters, IReadOnlyList<TypeSymbol> tacitParameters)
        {
            var key = new InstanceKey { Routine = routine, Parameters = parameters, TacitParameters = tacitParameters };
            if (!TemplateDictonary.ContainsKey(key))
            {
                return null;
            }
            return TemplateDictonary[key];
        }

        private struct InstanceKey : IEquatable<InstanceKey>
        {
            public RoutineSymbol Routine { get; set; }
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
                return Routine.Equals(other.Routine) &&
                    Parameters.SequenceEqual(other.Parameters) &&
                    TacitParameters.SequenceEqual(other.TacitParameters);
            }

            public override int GetHashCode()
            {
                var hash = Routine.GetHashCode();
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
