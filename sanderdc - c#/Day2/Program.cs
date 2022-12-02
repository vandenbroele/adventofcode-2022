IEnumerable<string> lines = File.ReadLines("./input.txt");
int totalScore = lines.Sum((s => ScorePart1.CalculateRound(s[0], s[2])));
Console.WriteLine($"1: {totalScore}");

lines = File.ReadLines("./input.txt");
int totalScore2 = lines.Sum((s => ScorePart2.CalculateRound(s[0], s[2])));
Console.WriteLine($"2: {totalScore2}");

public static class ScorePart1
{
    public static int CalculateRound(char other, char self)
    {
        return CalculateHand(self) + CalculateRoundOutcome(other, self);
    }

    public static int CalculateHand(char hand) => hand switch
    {
        'A' => 1,
        'B' => 2,
        'C' => 3,
        'X' => 1,
        'Y' => 2,
        'Z' => 3,
        _ => throw new ArgumentOutOfRangeException(nameof(hand), hand, null)
    };

    public static int CalculateRoundOutcome(char other, char self)
    {
        return other switch
        {
            'A' => self switch
            {
                'X' => 3,
                'Y' => 6,
                'Z' => 0,
                
                'A' => 3,
                'B' => 6,
                'C' => 0,
                _ => throw new InvalidDataException()
            },
            'B' => self switch
            {
                'Y' => 3,
                'Z' => 6,
                'X' => 0,
                
                'B' => 3,
                'C' => 6,
                'A' => 0,
                _ => throw new InvalidDataException()
            },
            'C' => self switch
            {
                'Z' => 3,
                'X' => 6,
                'Y' => 0,
                
                'C' => 3,
                'A' => 6,
                'B' => 0,
                _ => throw new InvalidDataException()
            },
            _ => throw new InvalidDataException()
        };
    }
}

public static class ScorePart2
{
    public static int CalculateRound(char other, char input)
    {
        return ScorePart1.CalculateRound(other, GetHand(other, input));
    }

    public static char GetHand(char hand, char input)
    {
        return hand switch
        {
            'A' => input switch
            {
                'X' => 'C',
                'Y' => 'A',
                'Z' => 'B',
                _ => throw new InvalidDataException()
            },
            'B'=> input switch
            {
                'X' => 'A',
                'Y' => 'B',
                'Z' => 'C',
                _ => throw new InvalidDataException()
            },
            'C'=> input switch
            {
                'X' => 'B',
                'Y' => 'C',
                'Z' => 'A',
                _ => throw new InvalidDataException()
            },
            _ => throw new InvalidDataException()
        };
    }
}