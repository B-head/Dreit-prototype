using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.Translate;

namespace Dlight
{
    class Module : Scope
    {
        public ExpressionList ExpList { get; set; }
        public List<Token> ErrorToken { get; set; }

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

        public override string ElementInfo()
        {
            return base.ElementInfo() + "ErrorToken = " + ErrorToken.Count;
        }

        public override void CheckSemantic()
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

        public override void SpreadTranslate(Translator trans)
        {
            Translator temp = trans.GenelateModule(Scope.FullName);
            base.SpreadTranslate(temp);
        }

        public override void Translate()
        {
            base.Translate();
            Scope scope = NameResolution("stdout");
            if(scope != null)
            {
                Trans.GenelateOperate(scope.FullName, TokenType.Special);
            }
        }
    }
}
