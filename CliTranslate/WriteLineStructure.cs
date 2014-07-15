﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliTranslate
{
    [Serializable]
    public class WriteLineStructure : CilStructure
    {
        public CilStructure Expression { get; private set; }

        public WriteLineStructure(CilStructure exp)
        {
            Expression = exp;
            AppendChild(Expression);
        }
    }
}