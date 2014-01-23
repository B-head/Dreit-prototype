using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    abstract class SyntaxOld
    {
        public SyntaxType Type { get; set; }
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

    class Token : SyntaxOld
    {
        public override string Text { get; set; }
        public override List<SyntaxOld> Child 
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override string ToString(int indent)
        {
            return Common.Indent(indent) + Position + ": " + Enum.GetName(typeof(SyntaxType), Type) + " => " + Text.Replace('\x0A', '\x20').Replace('\x0D', '\x20') + "\n";
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
            result.AppendLine(Common.Indent(indent) + Position + ": " + Enum.GetName(typeof(SyntaxType), Type));
            for (int i = 0; i < Child.Count; i++)
            {
                result.Append(Child[i].ToString(indent + 1));
            }
            return result.ToString();
        }
    }
}
