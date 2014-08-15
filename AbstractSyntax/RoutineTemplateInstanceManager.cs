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
        private Dictionary<RoutineSymbol, List<RoutineTemplateInstance>> TemplateDictonary;

        public RoutineTemplateInstanceManager()
        {
            TemplateDictonary = new Dictionary<RoutineSymbol, List<RoutineTemplateInstance>>();
        }

        public RoutineTemplateInstance Issue(RoutineSymbol routine, IReadOnlyList<TypeSymbol> parameter)
        {
            var ret = FindInstance(routine, parameter);
            if (ret != null)
            {
                return ret;
            }
            ret = new RoutineTemplateInstance(routine, parameter);
            AppendInstance(ret);
            return ret;
        }

        private void AppendInstance(RoutineTemplateInstance instance)
        {
            if(!TemplateDictonary.ContainsKey(instance.Routine))
            {
                TemplateDictonary.Add(instance.Routine, new List<RoutineTemplateInstance>());
            }
            TemplateDictonary[instance.Routine].Add(instance);
            AppendChild(instance);
        }

        private RoutineTemplateInstance FindInstance(RoutineSymbol routine, IReadOnlyList<TypeSymbol> parameter)
        {
            if (!TemplateDictonary.ContainsKey(routine))
            {
                return null;
            }
            var list = TemplateDictonary[routine];
            var ret = list.FirstOrDefault(v => v.Parameters.SequenceEqual(parameter));
            return ret;
        }
    }
}
