IEnumerable<string> lines = File.ReadLines("./input.txt");

List<int> topThree = ElfCounter
    .CreateCalorieCounts(lines)
    .OrderDescending()
    .Take(3)
    .ToList();

Console.WriteLine($"answer 1: {topThree.First()}");
Console.WriteLine($"answer 2: {topThree.Sum()}");

internal static class ElfCounter
{
    public static IEnumerable<int> CreateCalorieCounts(IEnumerable<string> lines)
    {
        int count = 0;
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                yield return count;
                count = 0;
            }
            else
            {
                count += int.Parse(line);
            }
        }
    }
}