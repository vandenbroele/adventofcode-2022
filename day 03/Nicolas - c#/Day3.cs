using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleApp1
{
    internal static class Day3
    {
        public static void Solution()
        {
            var totaal = 0;
            var input = File.ReadAllText("Day3.txt");
            var rugsacks = input.Split(Environment.NewLine).ToList();

            foreach (var rugsack in rugsacks)
            {
                var comp1Items = rugsack.Substring(0, rugsack.Length / 2).ToList();
                var comp2Items = rugsack.Substring((rugsack.Length / 2)).ToList();


                var items = comp1Items.Select(_ => _).Where(_ => comp2Items.Any(x => x == _)).Distinct().ToList();

                foreach (var item in items)
                {
                    int score = item;
                    if (score >= 97)
                        score -= 96;
                    else
                        score -= 38;

                    totaal += score;
                }

            }

            Console.WriteLine($"Day3 : {totaal}");

        }

        public static void Solution_plus()
        {
            var totaal = 0;
            var input = File.ReadAllText("Day3.txt");
            


            var teller = 1;
            var rugsacks = new List<string>();
            var groups = new List<List<string>>();

            foreach (var rugsack in input.Split(Environment.NewLine).ToList())
            {
                rugsacks.Add(rugsack);
                if (teller == 3)
                {
                    groups.Add(rugsacks);
                    rugsacks = new List<string>();
                    teller = 0;
                }
                teller++;
            }


            foreach (var groupRugsacks in groups)
            {

                var comp1Items = groupRugsacks[0];
                var comp2Items = groupRugsacks[1];
                var comp3Items = groupRugsacks[2];
 
                var items = comp3Items.Select(_ => _).Where(y => comp1Items.Select(_ => _).Where(_ => comp2Items.Any(x => x == _)).Distinct().ToList().Any(z => z == y)).Distinct().ToList();

                foreach (var item in items)
                {
                    int score = item;
                    if (score >= 97)
                        score -= 96;
                    else
                        score -= 38;

                    totaal += score;
                }
            }
            
            Console.WriteLine($"Day3+: {totaal}");

        }
    }
}
