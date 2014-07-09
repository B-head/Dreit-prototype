using AbstractSyntax.Directive;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateModule : NameSpaceSymbol
    {
        private DirectiveList Exp;
        public string SourceText { get; private set; }
        public IReadOnlyList<Token> ErrorToken { get; private set; }

        public DeclateModule(TextPosition tp, DirectiveList exp, string name, string source, IReadOnlyList<Token> error)
            :base(tp)
        {
            Exp = exp;
            Name = name;
            SourceText = source;
            ErrorToken = error;
            AppendChild(Exp);
        }

        internal override void CheckSemantic(CompileMessageManager cmm)
        {
            foreach (Token v in ErrorToken)
            {
                if (v.TokenType == TokenType.OtherString)
                {
                    cmm.CompileError("invalid-token", v);
                }
                else
                {
                    cmm.CompileError("error-token", v);
                }
            }
        }
    }
}
