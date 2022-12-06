using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleApp1
{
    internal static class Day6
    {
        public static void Solution()
        {
            var input = File.ReadAllText("Day6.txt").ToCharArray().ToList();

            var x = input.Select((_, i) => (_, i)).Where(_ => _.i >= 4 && input.Skip(_.i - 4).Take(4).Distinct().Count() == 4).FirstOrDefault().i;
            var y = input.Select((_, i) => (_, i)).Where(_ => _.i >= 14 && input.Skip(_.i - 14).Take(14).Distinct().Count() == 14).FirstOrDefault().i;
            Console.WriteLine($"Day6 : {x}");
            Console.WriteLine($"Day6+: {y}");



        }
    }
}
