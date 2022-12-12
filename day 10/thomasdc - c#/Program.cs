Console.WriteLine((
    from signal in EnumerateSignals()
    where signal.cycleNumber is 20 or 60 or 100 or 140 or 180 or 220
    select signal.cycleNumber * signal.x).Sum());

foreach (var (cycleNumber, x) in EnumerateSignals())
{
    var spritePosition = x - 1;
    Console.Write(cycleNumber % 40 - spritePosition is 1 or 2 or 3 ? '█' : ' ');
    if (cycleNumber % 40 == 0)
    {
        Console.WriteLine();
    }
}

IEnumerable<(int cycleNumber, int x)> EnumerateSignals()
{
    var x = 0;
    var nextX = 1;
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

        yield return (cycleNumber, x);
    }
}