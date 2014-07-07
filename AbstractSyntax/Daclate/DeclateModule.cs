using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateModule : NameSpace
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

        internal override void CheckSemantic()
        {
            foreach (Token v in ErrorToken)
            {
                if (v.TokenType == TokenType.OtherString)
                {
                    CompileError("invalid-token", v);
                }
                else
                {
                    CompileError("error-token", v);
                }
            }
            base.CheckSemantic();
        }

        private void CompileError(string key, Token token)
        {
            CompileMessage info = new CompileMessage
            {
                MessageType = CompileMessageType.Error,
                Key = key,
                Position = token.Position,
                Target = token,
            };
            Root.MessageManager.Append(info);
        }
    }
}
