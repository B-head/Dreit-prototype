using AbstractSyntax.Declaration;
using AbstractSyntax.Statement;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Expression
{
    [Serializable]
    public class ProgramContext : Element
    {
        public bool IsInline { get; private set; }

        public ProgramContext()
        {

        }

        public ProgramContext(TextPosition tp, IReadOnlyList<Element> child, bool isInline)
            :base(tp)
        {
            IsInline = isInline;
            AppendChild(child);
        }

        public void Append(Element child)
        {
            AppendChild(child);
        }

        public override bool IsConstant
        {
            get
            {
                foreach (var v in this)
                {
                    if (!v.IsConstant)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool IsNoReturn
        {
            get 
            { 
                if(Parent is IfStatement || Parent is LoopStatement)
                {
                    return true;
                }
                return Parent is NameSpaceSymbol && !(Parent is ModuleDeclaration); 
            }
        }
    }
}
