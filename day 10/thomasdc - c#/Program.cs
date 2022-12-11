int cycleNumber;
var nextCycleNumber = 1;
int x;
var nextX = 1;
var signalStrength = 0;
foreach (var line in File.ReadAllLines("input.txt"))
{
    x = nextX;
    cycleNumber = nextCycleNumber;

    Console.WriteLine($"cycle {cycleNumber} starting at {cycleNumber} and ending {cycleNumber + 1} has value {x} in the middle");

    if (cycleNumber is 20 or 60 or 100 or 140 or 180 or 220)
    {
        signalStrength += cycleNumber * x;
    }

    nextX = line[..4] switch
    {
        "addx" => x + int.Parse(line[5..]),
        _ => x
    };

    nextCycleNumber = line[..4] switch
    {
        "addx" => cycleNumber + 2,
        _ => cycleNumber + 1
    };

    if (nextCycleNumber is 21 or 61 or 101 or 141 or 181 or 221 && cycleNumber is 19 or 59 or 99 or 139 or 179 or 219)
    {
        signalStrength += (cycleNumber + 1) * x;
    }
}

Console.WriteLine(signalStrength);