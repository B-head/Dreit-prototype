using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight
{
    class AbstractSyntax : IEnumerable<AbstractSyntax>
    {
        public TextPosition Position { get; set; }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual AbstractSyntax this[int index]
        {
            get { throw new ArgumentOutOfRangeException(); }
        }

        public string BasicInfo()
        {
            return Position + " " + this.GetType().Name + ": ";
        }

        public virtual string Info()
        {
            return String.Empty;
        }

        public string ErrorInfo()
        {
            return "Error: " + Position + ": ";
        }

        public virtual void CheckError(TextWriter output)
        {
            foreach (AbstractSyntax v in this)
            {
                if (v == null)
                {
                    output.WriteLineAsync(ErrorInfo() + "コンパイラの意図しない<null>値である子要素が検出されました。");
                    continue;
                }
                v.CheckError(output);
            }
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public virtual string ToString(int indent)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(Common.Indent(indent) + BasicInfo() + Info());
            foreach (AbstractSyntax v in this)
            {
                if(v == null)
                {
                    result.AppendLine("<null>");
                    continue;
                }
                result.Append(v.ToString(indent + 1));
            }
            return result.ToString();
        }

        public IEnumerator<AbstractSyntax> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class Root : AbstractSyntax
    {
        public IReadOnlyList<AbstractSyntax> Child { get; set; }
        public IReadOnlyList<Token> ErrorToken { get; set; }

        public override int Count
        {
            get { return Child.Count; }
        }

        public override AbstractSyntax this[int index]
        {
            get { return Child[index]; }
        }

        public override string Info()
        {
            return "Error " + ErrorToken.Count.ToString();
        }
    }

    class Binomial : AbstractSyntax
    {
        public AbstractSyntax Left { get; set; }
        public AbstractSyntax Right { get; set; }
        public SyntaxType Operation { get; set; }

        public override int Count
        {
            get { return 2; }
        }

        public override AbstractSyntax this[int index]
        {
            get
            { 
                switch(index)
                {
                    case 0: return Left;
                    case 1: return Right;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string Info()
        {
            return Enum.GetName(typeof(SyntaxType), Operation);
        }
    }
}
