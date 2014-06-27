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

    interface IAccess : IElement
    {
        OverLoad Reference { get; }
        void RefarenceResolution();
        void RefarenceResolution(IScope scope);
    }

    interface ICaller : IElement
    {
        bool HasCallTarget(IElement element);
        IDataType GetCallType();
    }
}
