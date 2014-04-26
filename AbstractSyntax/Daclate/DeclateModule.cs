using AbstractSyntax.Visualizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AbstractSyntax.Daclate
{
    [Serializable]
    public class DeclateModule : Scope
    {
        public string SourceText { get; set; }
        public DirectiveList ExpList { get; set; }
        public List<Token> ErrorToken { get; set; }

        public DeclateModule()
        {
            ExpList = new DirectiveList();
            ErrorToken = new List<Token>();
        }

        public void Append(Element append)
        {
            ExpList.Append(append);
        }

        public override int Count
        {
            get { return 1; }
        }

        public override Element GetChild(int index)
        {
            switch (index)
            {
                case 0: return ExpList;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal override bool IsNameSpace
        {
            get { return true; }
        }

        internal override void CheckSyntax()
        {
            foreach (Token v in ErrorToken)
            {
                //todo エラーオブジェクトにTokenを渡す必要がある。
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
