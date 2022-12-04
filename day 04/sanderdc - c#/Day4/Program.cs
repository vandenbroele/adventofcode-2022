int count = File.ReadLines("./input.txt")
    .Select(ToRanges)
    .Count(CheckOverlap);

Console.WriteLine($"part 1: {count}");

int count2 = File.ReadLines("./input.txt")
    .Select(ToRanges)
    .Count(CheckPartialOverlap);

Console.WriteLine($"part 2: {count2}");


(Range first, Range second) ToRanges(string s)
{
    string[] assignments = s.Split(',');
    Range range = Range.Parse(assignments[0]);
    Range second1 = Range.Parse(assignments[1]);
    return (range, second1);
}

bool CheckOverlap((Range first, Range second) input)
{
    return Range.FullOverlap(input.first, input.second);
}

bool CheckPartialOverlap((Range first, Range second) input)
{
    return Range.PartialOverLap(input.first, input.second);
}


public class Range
{
    public int Min;
    public int Max;

    public static Range Parse(string input)
    {
        string[] stuff = input.Split('-');
        return new Range
        {
            Min = int.Parse(stuff[0]),
            Max = int.Parse(stuff[1])
        };
    }

    public static bool FullOverlap(Range r1, Range r2)
    {
        bool overlaps1 = r1.Min >= r2.Min && r1.Max <= r2.Max;
        bool overlaps2 = r2.Min >= r1.Min && r2.Max <= r1.Max;

        return overlaps1 || overlaps2;
    }

    public static bool PartialOverLap(Range r1, Range r2)
    {
        return (r1.Min >= r2.Min && r1.Min <= r2.Max)
            || (r1.Max >= r2.Min && r1.Max <= r2.Max)
            || (r2.Min >= r1.Min && r2.Min <= r1.Max)
            || (r2.Max >= r1.Min && r2.Max <= r1.Max);
    }
}

