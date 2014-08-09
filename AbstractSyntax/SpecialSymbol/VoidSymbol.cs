﻿using AbstractSyntax.Visualizer;
using System;
using System.Diagnostics;

namespace AbstractSyntax.SpecialSymbol
{
    [Serializable]
    public class VoidSymbol : Scope
    {
        public VoidSymbol()
        {
            Name = "void";
        }

        public override bool IsDataType
        {
            get { return true; }
        }
    }
}