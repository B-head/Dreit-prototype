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
        public List<Element> Child { get; set; }
        public List<Token> ErrorToken { get; set; }

        public override int ChildCount
        {
            get { return Child.Count; }
        }

        public override Element GetChild(int index)
        {
            return Child[index];
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
            Translator temp = trans.GenelateModule(Scope);
            base.SpreadTranslate(temp);
        }
    }
}
