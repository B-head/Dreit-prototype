using AbstractSyntax.Expression;
using AbstractSyntax.Symbol;
using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class ModuleDeclaration : NameSpaceSymbol
    {
        public ExpressionList Directives { get; private set; }
        public string SourceText { get; private set; }
        public IReadOnlyList<Token> ErrorToken { get; private set; }

        public ModuleDeclaration(TextPosition tp, ExpressionList drcs, string name, string source, IReadOnlyList<Token> error)
            :base(tp)
        {
            Directives = drcs;
            Name = name;
            SourceText = source;
            ErrorToken = error;
            AppendChild(Directives);
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
