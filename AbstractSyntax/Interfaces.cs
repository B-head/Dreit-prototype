using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax
{
    public interface IPragma
    {

    }

    interface IAccess
    {
        OverLoad Reference { get; }
        void RefarenceResolution(Scope scope);
    }

    interface ICaller
    {
        bool HasCallTarget(Element element);
        DataType GetCallType();
    }
}
