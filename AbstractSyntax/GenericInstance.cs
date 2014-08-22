/*
Copyright 2014 B_head

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
    public struct GenericsInstance
    {
        public GenericSymbol Generic { get; set; }
        public TypeSymbol Type { get; set; }

        public static IReadOnlyList<GenericsInstance> MakeGenericInstance(IReadOnlyList<GenericSymbol> generics, IReadOnlyList<TypeSymbol> types)
        {
            var ret = new List<GenericsInstance>();
            for (var i = 0; i < generics.Count; ++i)
            {
                if (i < types.Count)
                {
                    var gi = new GenericsInstance { Generic = generics[i], Type = types[i] };
                    ret.Add(gi);
                }
                else
                {
                    var gi = new GenericsInstance { Generic = generics[i], Type = generics[i] };
                    ret.Add(gi);
                }
            }
            return ret;
        }

        public static int FindGenericIndex(IReadOnlyList<GenericsInstance> inst, GenericSymbol generic)
        {
            for (var i = 0; i < inst.Count; ++i)
            {
                if (inst[i].Generic == generic)
                {
                    return i;
                }
            }
            return -1;
        }

        public static IReadOnlyList<TypeSymbol> MakeClassTemplateInstanceList(Root root, IReadOnlyList<GenericsInstance> inst, IReadOnlyList<TypeSymbol> baseTypes)
        {
            var ret = new List<TypeSymbol>();
            foreach(var v in baseTypes)
            {
                ret.Add(MakeClassTemplateInstance(root, inst, v));
            }
            return ret;
        }

        public static IReadOnlyList<ArgumentSymbol> MakeArgumentTemplateInstanceList(Root root, IReadOnlyList<GenericsInstance> inst, IReadOnlyList<ArgumentSymbol> args)
        {
            var ret = new List<ArgumentSymbol>();
            foreach (var v in args)
            {
                var t = MakeClassTemplateInstance(root, inst, v.DataType);
                var p = new ArgumentSymbol();
                p.Initialize(v.Name, v.VariantType, v.Attribute, t);
                ret.Add(p);
            }
            return ret;
        }

        public static TypeSymbol MakeClassTemplateInstance(Root root, IReadOnlyList<GenericsInstance> inst, TypeSymbol baseType)
        {
            var g = baseType as GenericSymbol;
            if(g != null)
            {
                var i = FindGenericIndex(inst, g);
                if (i == -1)
                {
                    return baseType;
                }
                return inst[i].Type;
            }
            else if (baseType.Generics.Count > 0 || baseType.TacitGeneric.Count > 0)
            {
                var prm = new List<TypeSymbol>();
                foreach (var v in baseType.Generics)
                {
                    var t = MakeClassTemplateInstance(root, inst, v);
                    prm.Add(t);
                }
                var tprm = new List<TypeSymbol>();
                foreach (var v in baseType.TacitGeneric)
                {
                    var t = MakeClassTemplateInstance(root, inst, v);
                    tprm.Add(t);
                }
                return root.ClassManager.Issue(baseType, prm, tprm);
            }
            else
            {
                return baseType;
            }
        }

        public static RoutineSymbol MakeRoutineTemplateInstance(Root root, IReadOnlyList<GenericsInstance> inst, RoutineSymbol baseRoutine)
        {
            if (baseRoutine.Generics.Count > 0 || baseRoutine.TacitGeneric.Count > 0)
            {
                var prm = new List<TypeSymbol>();
                foreach (var v in baseRoutine.Generics)
                {
                    var t = MakeClassTemplateInstance(root, inst, v);
                    prm.Add(t);
                }
                var tprm = new List<TypeSymbol>();
                foreach (var v in baseRoutine.TacitGeneric)
                {
                    var t = MakeClassTemplateInstance(root, inst, v);
                    tprm.Add(t);
                }
                return root.RoutineManager.Issue(baseRoutine, prm, tprm);
            }
            else
            {
                return baseRoutine;
            }
        }
    }
}
