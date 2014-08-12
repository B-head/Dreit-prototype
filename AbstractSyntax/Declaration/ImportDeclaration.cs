using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSyntax.Declaration
{
    [Serializable]
    public class ImportDeclaration : Scope
    {
        public IReadOnlyList<Element> Imports { get; private set; }

        public ImportDeclaration(TextPosition tp, IReadOnlyList<Element> imports)
            :base(tp)
        {
            Imports = imports;
            AppendChild(Imports);
        }
    }
}
