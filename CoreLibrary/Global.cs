using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public static class Global
    {
        public static void stdout(params object[] puts)
        {
            for(var i = 0; i < puts.Length; ++i)
            {
                if(i > 0)
                {
                    Console.Write(" ");
                }
                Console.Write(puts[i]);
            }
            Console.WriteLine();
        }
    }
}
