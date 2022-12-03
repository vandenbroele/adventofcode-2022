int count1 = File.ReadLines("./input.txt")
    .Select(Helpers.GetOverlap)
    .Sum(Helpers.GetCharPriority);

Console.WriteLine($"part 1: {count1}");

int count2 = File.ReadLines("./input.txt")
    .Chunk(3)
    .Select(Helpers.GetOverlap)
    .Sum(Helpers.GetCharPriority);

Console.WriteLine($"part 2: {count2}");

public static class Helpers
{
    public static char GetOverlap(IEnumerable<string> inputs)
    {
        using IEnumerator<string> enumerator = inputs.GetEnumerator();

        enumerator.MoveNext();
        string start = enumerator.Current;
        IEnumerable<char> current = start;

        while (enumerator.MoveNext())
        {
            current = current.Intersect(enumerator.Current);
        }

        return current.First();
    }

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