var x = 0;
var nextX = 1;
var signalStrength = 0;
var nextCycleNumber = 1;
var lines = File.ReadAllLines("input.txt");
var lineI = 0;
for (var cycleNumber = 1; cycleNumber <= 240; cycleNumber++)
{
    if (cycleNumber == nextCycleNumber)
    {
        x = nextX;

        nextX = lines[lineI][..4] switch
        {
            "addx" => x + int.Parse(lines[lineI][5..]),
            _ => x
        };

        nextCycleNumber = lines[lineI][..4] switch
        {
            "addx" => cycleNumber + 2,
            _ => cycleNumber + 1
        };

        lineI++;
    }

    if (cycleNumber is 20 or 60 or 100 or 140 or 180 or 220)
    {
        signalStrength += cycleNumber * x;
    }
}

Console.WriteLine(signalStrength);