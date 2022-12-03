int count = File.ReadLines("./input.txt")
    .Select(Helpers.GetOverlap)
    .Sum(Helpers.GetCharPriority);

Console.WriteLine($"part 1: {count}");

int count2 = File.ReadLines("./input.txt")
    .Chunk()
    .Select(i => Helpers.GetOverlap(i.first, i.second, i.third))
    .Sum(Helpers.GetCharPriority);

Console.WriteLine($"part 2: {count2}");

public static class Helpers
{
    public static IEnumerable<(string first, string second, string third)> Chunk(this IEnumerable<string> input)
    {
        using IEnumerator<string> enumerator = input.GetEnumerator();

        // We cheat here by assuming multiples of three
        bool hasNext;
        do
        {
            (string first, string second, string third) output;
            hasNext = enumerator.MoveNext();
            if (!hasNext)
                break;

            output.first = enumerator.Current;
            enumerator.MoveNext();
            output.second = enumerator.Current;

            hasNext = enumerator.MoveNext();
            output.third = enumerator.Current;
            yield return output;
        } while (hasNext);
    }


    public static char GetOverlap(string first, string second, string third)
    {
        return first.Intersect(second).Intersect(third).First();
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