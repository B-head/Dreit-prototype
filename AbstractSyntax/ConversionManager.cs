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
    class ConversionManager
    {
        private Root Root;
        private List<RoutineSymbol> ConvList;

        public ConversionManager(Root root)
        {
            Root = root;
            ConvList = new List<RoutineSymbol>();
        }

        public void Append(RoutineSymbol symbol)
        {
            ConvList.Add(symbol);
        }

        public IEnumerable<RoutineSymbol> GetAllInitializer(TypeSymbol type)
        {
            return ConvList.FindAll(v => v.CallReturnType == type);
        }

        public RoutineSymbol Find(TypeSymbol from, TypeSymbol to)
        {
            var s = ConvList.FindAll(v => v.CallReturnType == to && v.Arguments[0].ReturnType == from);
            if(s.Count == 1)
            {
                return s[0];
            }
            else if(s.Count > 1)
            {
                return Root.ErrorRoutine;
            }
            else
            {
                if (ContainSubType(from, to))
                {
                    return Root.Default;
                }
                else
                {
                    return Root.ErrorRoutine;
                }
            }
        }

        private static bool ContainSubType(TypeSymbol from, TypeSymbol to)
        {
            foreach(var v in from.EnumSubType())
            {
                if(v == to)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
