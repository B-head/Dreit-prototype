using AbstractSyntax.Daclate;
using AbstractSyntax.Pragma;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class CallRoutine : Caller
    {
        public Element CallAccess { get; set; }
        public TupleList CallArguments { get; set; }

        public override Element Access
        {
            get { return CallAccess; }
        }

        public override TupleList Arguments
        {
            get { return CallArguments; }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override IElement this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return CallAccess;
                    case 1: return CallArguments;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override bool HasCallTarget(IElement element)
        {
            return CallAccess == element;
        }

        public override IDataType GetCallType()
        {
            if (CallArguments.GetDataTypes().Count != 1)
            {
                return Root.Unknown;
            }
            return CallArguments.GetDataTypes()[0];
        }
    }
}
