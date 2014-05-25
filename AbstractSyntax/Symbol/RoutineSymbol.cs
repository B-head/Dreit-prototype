﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Symbol
{
    [Serializable]
    public class RoutineSymbol : Scope
    {
        public TokenType Operator { get; set; }
        protected List<IDataType> _ArgumentType;
        protected IDataType _ReturnType;

        public virtual List<IDataType> ArgumentType
        {
            get { return _ArgumentType; }
        }

        public virtual IDataType ReturnType
        {
            get { return _ReturnType; }
        }

        public override IDataType DataType
        {
            get { return ReturnType; }
        }

        internal override IEnumerable<TypeMatch> GetTypeMatch(IReadOnlyList<IDataType> type)
        {
            yield return TypeMatch.MakeTypeMatch(Root.Conversion, this, type, ArgumentType);
        }
    }
}
