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

    interface IAccess
    {
        OverLoad Reference { get; }
        void RefarenceResolution(IScope scope);
    }

    interface ICaller
    {
        bool HasCallTarget(IElement element);
        IDataType GetCallType();
    }
}
