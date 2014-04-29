using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateModule : NameSpace
    {
        public string SourceText { get; set; }
        public IReadOnlyList<Token> ErrorToken { get; set; }

        public DeclateModule()
        {
            ErrorToken = new List<Token>();
        }

        internal override void CheckSyntax()
        {
            foreach (Token v in ErrorToken)
            {
                if (v.Type == TokenType.OtherString)
                {
                    CompileError("invalid-token", v);
                }
                else
                {
                    CompileError("error-token", v);
                }
            }
            base.CheckSyntax();
        }

        private void CompileError(string key, Token token)
        {
            CompileMessage info = new CompileMessage
            {
                Type = CompileMessageType.Error,
                Key = key,
                Position = token.Position,
                Target = token,
            };
            Root.MessageManager.Append(info);
        }
    }
}
