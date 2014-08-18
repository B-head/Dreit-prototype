using AbstractSyntax.Expression;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Literal
{
    [Serializable]
    public class TupleLiteral : Element
    {
        public TupleLiteral()
        {

        }

        public TupleLiteral(TextPosition tp, Element append)
            : base(tp)
        {
            AppendChild(append);
        }

        public TupleLiteral(TextPosition tp, List<Element> child)
            :base(tp)
        {
            AppendChild(child);
        }

        public override bool IsConstant
        {
            get 
            {
                foreach(var v in this)
                {
                    if(!v.IsConstant)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
