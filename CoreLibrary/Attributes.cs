using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class GlobalScopeAttribute : Attribute
    {

    }
}
