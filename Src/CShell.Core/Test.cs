using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell
{
    public static class Test
    {
        public static string Txt { get; set; }
        public static void Print()
        {
            Console.WriteLine(Txt);
        }
    }
}
