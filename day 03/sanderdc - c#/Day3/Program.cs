IEnumerable<string> lines = File.ReadLines("./input.txt");

int count = lines
    .Select(Helpers.GetOverlap)
    .Sum(Helpers.GetCharPriority);

Console.WriteLine(count);

public static class Helpers
{
    public static char GetOverlap(string input)
    {
        int half = input.Length / 2;
        string left = input[..half];
        string right = input[half..];

        return left.Intersect(right).First();
    }

    public static int GetCharPriority(char input)
    {
        if (char.IsUpper(input))
        {
            return 27 + input - 65;
        }

        return input - 96;
    }
}