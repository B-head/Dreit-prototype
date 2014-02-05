using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlight.CilTranslate;

namespace Dlight
{
    class ModuleFile : Element
    {
        public string Name { get; set; }
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

        protected override string ElementInfo()
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

        protected override Translator CreateTranslator(Translator trans)
        {
            return trans.CreateNameSpace(Name);
        }

        public override void Translate()
        {
            base.Translate();
            //Trans.GenelateControl(VirtualCodeType.Return);
        }
    }
}
