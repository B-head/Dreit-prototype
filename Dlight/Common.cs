using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlight
{
    static class Common
    {
        public static void CompileError(string message, string file, int line)
        {
            Console.WriteLine("Error " + file + "(" + line + "): " + message);
        }
    }

    struct Token
    {
        public string Value;
        public string File;
        public int Line;

        public override string ToString()
        {
            return File + "(" + Line + "): " + Value;
        }
    }
}
