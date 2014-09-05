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
using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class CastSymbol : RoutineSymbol
    {
        public PrimitiveType PrimitiveType { get; private set; }

        public CastSymbol(PrimitiveType type, ClassSymbol from, ClassSymbol to)
            :base(RoutineType.FunctionConverter, TokenType.Unknoun)
        {
            Name = to.Name;
            PrimitiveType = type;
            _Arguments = ArgumentSymbol.MakeParameters(from);
            _CallReturnType = to;
        }
    }

    public enum PrimitiveType
    {
        NotPrimitive = 0,
        Integer8 = 1,
        Integer16 = 3,
        Integer32 = 5,
        Integer64 = 7,
        Natural8 = 2,
        Natural16 = 4,
        Natural32 = 6,
        Natural64 = 8,
        Binary32 = 9,
        Binary64 = 10,
    }
}
