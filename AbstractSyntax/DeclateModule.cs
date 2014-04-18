using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractSyntax;

namespace AbstractSyntax
{
    public class DeclateModule : Scope
    {
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
                if (v.Type == TokenType.OtherString)
                {
                    CompileError(": 文字列 " + v.Text + " は有効なトークンではありません。", v.Position);
                }
                else
                {
                    CompileError(": トークン " + v.Text + " をこの位置に書くことは出来ません。", v.Position);
                }
            }
            base.CheckSyntax();
        }
    }
}
