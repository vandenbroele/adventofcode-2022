IEnumerable<string> lines = File.ReadLines("./input.txt");

int totalScore = lines.Sum((s => Score.CalculateRound(s[0], s[2])));

Console.WriteLine($"1: {totalScore}");

public static class Score
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
                _ => throw new InvalidDataException()
            },
            'B' => self switch
            {
                'Y' => 3,
                'Z' => 6,
                'X' => 0,
                _ => throw new InvalidDataException()
            },
            'C' => self switch
            {
                'Z' => 3,
                'X' => 6,
                'Y' => 0,
                _ => throw new InvalidDataException()
            },
            _ => throw new InvalidDataException()
        };
    }
}