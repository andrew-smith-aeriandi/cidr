using System;
using System.Linq;

namespace Cidr
{
    class Program
    {
        static void Main(string[] args)
        {
            var cidrs = args
                .ParseCidrs()
                .ConsolidateCidrs();

            foreach (var cidr in cidrs)
            {
                Console.WriteLine(cidr.ToString());
            }
        }
    }
}
