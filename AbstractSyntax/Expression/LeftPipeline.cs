using AbstractSyntax.Declaration;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class LeftPipeline : CallExpression
    {
        public TokenType Operator { get; set; }

        public LeftPipeline(TextPosition tp, TokenType op, Element left, Element right)
            :base(tp, left, right)
        {
            Operator = op;
        }

        public override Element Left
        {
            get
            {
                return Access;
            }
        }

        public override Element Right
        {
            get
            {
                return (Element)Arguments[0];
            }
        }

        public override TokenType CalculateOperator
        {
            get { return Operator ^ TokenType.LeftPipeline; }
        }

        protected override string ElementInfo
        {
            get { return Operator.ToString(); }
        }

        public override bool IsPipeline
        {
            get { return true; }
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            if(Right != null && Right is RightPipeline)
            {
                cmm.CompileError("not-collide-assign", this);
            }
            if (Left != null && Left is RightPipeline)
            {
                cmm.CompileError("not-collide-assign", this);
            }
            base.CheckSemantic(cmm);
        }
    }
}
