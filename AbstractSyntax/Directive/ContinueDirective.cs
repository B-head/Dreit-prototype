﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Directive
{
    [Serializable]
    public class ContinueDirective : Element
    {
        public ContinueDirective(TextPosition tp)
            :base(tp)
        {

        }
    }
}
