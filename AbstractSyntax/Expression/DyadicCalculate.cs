using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;
using System.Linq;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class DyadicCalculate : DyadicExpression
    {
        public Scope CallScope { get; private set; }

        public override DataType DataType
        {
            get { return Left.DataType; }
        }

        internal override void CheckDataType()
        {
            base.CheckDataType();
            DataType l = Left.DataType;
            DataType r = Right.DataType;
            if (l != r && !(l is UnknownSymbol) && !(r is UnknownSymbol))
            {
                CompileError("impossible-calculate");
            }
            string callName = string.Empty;
            switch(Operator)
            {
                case TokenType.Add: callName = "+"; break;
                case TokenType.Subtract: callName = "-"; break;
                case TokenType.Multiply: callName = "*"; break;
                case TokenType.Divide: callName = "/"; break;
                case TokenType.Modulo: callName = "%"; break;
                default: throw new InvalidOperationException();
            }
            var ol = l.NameResolution(callName);
            CallScope = ol.TypeSelect(new DataType[] { r }.ToList()).Call;
        }
    }
}
