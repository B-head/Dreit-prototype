using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    abstract class SyntaxOld
    {
        public TokenType Type { get; set; }
        public TextPosition Position { get; set; }
        public abstract string Text { get; set; }
        public abstract List<SyntaxOld> Child { get; set; }

        public override string ToString()
        {
            return ToString(0);
        }

        public virtual string ToString(int indent)
        {
            return base.ToString();
        }
    }

    class DirectiveOld : SyntaxOld
    {
        public override string Text
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public override List<SyntaxOld> Child { get; set; }

        public override string ToString(int indent)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(Position + ": " + Enum.GetName(typeof(TokenType), Type));
            for (int i = 0; i < Child.Count; i++)
            {
                result.Append(Child[i].ToString(indent + 1));
            }
            return result.ToString();
        }
    }
}
