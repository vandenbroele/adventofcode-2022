using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleApp1
{
    internal static class Day4
    {
        public static void Solution()
        {
            var input = File.ReadAllText("Day4.txt");
            var groups = input.Split(Environment.NewLine).ToList()
                              .Select(_ => _.Split(",").Select(x => new { Start = int.Parse(x.Split("-")[0])
                                                                            , End = int.Parse(x.Split("-")[1]) }).ToList()).ToList();


            var volledigeOverlaps = groups.Select(_ => _).Where(_ => (_[0].Start <= _[1].Start && _[0].End >= _[1].End)
                                                                  || (_[1].Start <= _[0].Start && _[1].End >= _[0].End)).Count();

            var overlaps = groups.Select(_ => _).Where(_ => (_[0].Start <= _[1].Start && _[0].End >= _[1].Start)
                                                                  || (_[0].Start <= _[1].End && _[0].End >= _[1].End)
                                                                  || (_[1].Start <= _[0].Start && _[1].End >= _[0].Start)
                                                                  || (_[1].Start <= _[0].End && _[1].End >= _[0].End)).Count();



            Console.WriteLine($"Day4 : {volledigeOverlaps}");
            Console.WriteLine($"Day4+: {overlaps}");

        }

    }
}
