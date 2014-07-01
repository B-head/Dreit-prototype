using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public interface IDataType : IScope
    {

    }

    public interface IMonadicExpression : IElement
    {
        Element Child { get; set; }
        TokenType Operator { get; set; }
    }

    public interface IDyadicExpression : IElement
    {
        Element Left { get; set; }
        Element Right { get; set; }
        TokenType Operator { get; set; }
    }
}
