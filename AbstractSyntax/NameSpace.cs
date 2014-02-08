using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTranslate;
using Common;

namespace AbstractSyntax
{
    public class NameSpace : Scope
    {
        public ExpressionList ExpList { get; set; }
        public List<Token> ErrorToken { get; set; }

        public NameSpace()
        {
            ErrorToken = new List<Token>();
        }

        public override int ChildCount
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

        protected override string ElementInfo()
        {
            return base.ElementInfo() + "ErrorToken = " + ErrorToken.Count;
        }

        internal override void CheckSemantic()
        {
            foreach (Token v in ErrorToken)
            {
                if (v.Type == TokenType.OtherString)
                {
                    CompileError(": 文字列 " + v.Text + " は有効なトークンではありません。");
                }
                else
                {
                    CompileError(": トークン " + v.Text + " をこの位置に書くことは出来ません。");
                }
            }
            base.CheckSemantic();
        }

        internal override Translator CreateTranslator(Translator trans)
        {
            return trans.CreateNameSpace(FullPath);
        }

        internal override void Translate()
        {
            base.Translate();
            //Trans.GenelateControl(VirtualCodeType.Return);
        }
    }
}
