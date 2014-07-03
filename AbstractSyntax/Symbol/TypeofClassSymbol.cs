﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class TypeofClassSymbol : Scope, IDataType
    {
        private ClassSymbol BaseClass;

        public TypeofClassSymbol(ClassSymbol baseClass)
        {
            BaseClass = baseClass;
        }

        public override IReadOnlyList<IScope> Attribute
        {
            get { return BaseClass.Attribute; }
        }
    }
}