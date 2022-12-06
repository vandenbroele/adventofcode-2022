using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal static class Day2
    {
        public static void Solution()
        {

            var input = File.ReadAllText("Day2.txt");
            var rounds = input.Split(Environment.NewLine);
            var score = 0; var score2 = 0;
            var totaal = 0; var totaal2 = 0;

            foreach (var round in rounds)
            {
                var hands = round.Split(" ");
                var shapeValue = new List<dynamic>();
                switch (hands[0])
                {
                    case "A": //steen
                        shapeValue.Add(new { Shape = "X", ShapeValue = 1, Points = 3, Score = "lose", ScorePoints = 0 });  //Steen
                        shapeValue.Add(new { Shape = "Y", ShapeValue = 2, Points = 6, Score = "draw", ScorePoints = 3 });  //blad
                        shapeValue.Add(new { Shape = "Z", ShapeValue = 3, Points = 0, Score = "win", ScorePoints = 6 });  //schaar
                        break;
                    case "B": //blad
                        shapeValue.Add(new { Shape = "X", ShapeValue = 1, Points = 0, Score = "lose", ScorePoints = 0 });  //Steen
                        shapeValue.Add(new { Shape = "Y", ShapeValue = 2, Points = 3, Score = "draw", ScorePoints = 3 });  //blad
                        shapeValue.Add(new { Shape = "Z", ShapeValue = 3, Points = 6, Score = "win", ScorePoints = 6 });  //schaar
                        break;
                    case "C": //schaar
                        shapeValue.Add(new { Shape = "X", ShapeValue = 1, Points = 6, Score = "lose", ScorePoints = 0 });  //Steen
                        shapeValue.Add(new { Shape = "Y", ShapeValue = 2, Points = 0, Score = "draw", ScorePoints = 3 });  //blad
                        shapeValue.Add(new { Shape = "Z", ShapeValue = 3, Points = 3, Score = "win", ScorePoints = 6 });  //schaar
                        break;
                }

                var shape = shapeValue.Select(_ => _).Where(_ => _.Shape == hands[1]).FirstOrDefault();

                var outcome = shapeValue.Select(_ => _).Where(_ => _.Shape == hands[1]).FirstOrDefault();
                var hand = shapeValue.Select(_ => _).Where(_ => _.Points == outcome?.ScorePoints).FirstOrDefault();


                score = shape?.ShapeValue + shape?.Points;
                totaal += score;

                score2 = hand?.ShapeValue + hand?.Points;
                totaal2 += score2;
            }

            Console.WriteLine($"Day2 : {totaal}");
            Console.WriteLine($"Day2+: {totaal2}");

        }
    }
}
