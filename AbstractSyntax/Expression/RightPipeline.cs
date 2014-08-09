using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class RightPipeline : CallExpression
    {
        public TokenType Operator { get; set; }

        public RightPipeline(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, right, left)
        {
            Operator = op;
        }

        public override Element Right
        {
            get
            {
                return Access;
            }
        }

        public override Element Left
        {
            get
            {
                return (Element)Arguments[0];
            }
        }

        public override TokenType CalculateOperator
        {
            get { return Operator ^ TokenType.RightPipeline; }
        }

        protected override string ElementInfo
        {
            get { return Operator.ToString(); }
        }
    }
}
