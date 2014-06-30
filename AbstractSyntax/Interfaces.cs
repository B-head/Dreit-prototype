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

    public interface IAttribute : IElement
    {
        IReadOnlyList<IScope> Attribute { get; }
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

    public interface IAccess : IElement
    {
        OverLoad Reference { get; }
        Scope CallScope { get; }
        void RefarenceResolution();
        void RefarenceResolution(IScope scope);
    }
}
