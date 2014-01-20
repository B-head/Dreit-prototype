using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dlight.SemanticAnalysis
{
    partial class Checker
    {
        public void Check(Root root, TextWriter output)
        {
            root.CheckError(output);
        }
    }
}
