Input[] inputs =
{
    new("./testInput.txt", 10, 20),
    new("./input.txt", 2000000, 4000000)
};

(string path, int rowToCheck, int maxSize) = inputs[0];

List<string> inputLines = File.ReadLines(path).ToList();

List<(Pos sensorPos, Pos beaconPos, int distance)> pairs = inputLines
    .Select(ParseLine)
    .ToList();

HashSet<int> positions = new();

foreach ((Pos sensorPos, Pos _, int distance) in pairs)
{
    Range range = GetRowOverlapRange(rowToCheck, sensorPos, distance);

    if (range.To - range.From <= 0)
    {
        continue;
    }

    for (int i = range.From; i <= range.To; i++)
    {
        positions.Add(i);
    }
}

foreach ((Pos _, Pos beaconPos, int _) in pairs)
{
    if (beaconPos.Y == rowToCheck)
    {
        positions.Remove(beaconPos.X);
    }
}

Console.WriteLine($"part 1: {positions.Count}");


List<Range> ranges = new();
for (int row = 0; row <= maxSize; row++)
{
    ranges.Clear();

    foreach ((Pos sensorPos, Pos _, int distance) in pairs)
    {
        Range range = GetRowOverlapRange(row, sensorPos, distance);

        if (range.To - range.From <= 0)
        {
            continue;
        }


        int min = Math.Max(range.From, 0);
        int max = Math.Min(range.To, maxSize);

        CombineRanges(ranges, new Range(min, max));
    }

    if (ranges.Count > 1)
    {
        int column = ranges[0].To + 1;
        long result = column * 4000000L + row;
        Console.WriteLine($"part 2: {result}");
    }
}

void CombineRanges(List<Range> list, Range range)
{
    if (list.Count <= 0)
    {
        list.Add(range);
        return;
    }

    // Assume ordered for code below

    for (int i = 0; i < list.Count; i++)
    {
        Range other = list[i];

        // Join adjacent
        if (range.To + 1 == other.From)
        {
            list[i] = new Range(range.From, other.To);
            return;
        }

        if (range.From - 1 == other.To)
        {
            list[i] = new Range(other.From, range.To);
            return;
        }

        if (other.To < range.From)
            continue;

        if (other.From > range.To)
        {
            list.Insert(i, range);
            return;
        }

        // Enveloped
        if (range.From >= other.From && range.To <= other.To)
        {
            return;
        }


        // Append
        if (range.From < other.From && range.To >= other.From && range.To <= other.To)
        {
            list[i] = new Range(range.From, other.To);
            return;
        }

        if (range.From >= other.From && range.From <= other.To && range.To > other.To)
        {
            Range combinedRange = new(other.From, range.To);
            list.RemoveAt(i);

            // Check further overlap
            CombineRanges(list, combinedRange);
            return;
        }


        // Full overlap
        if (other.From > range.From && other.To < range.To)
        {
            // Check further overlap

            list.RemoveAt(i);
            CombineRanges(list, range);
            return;
        }
    }


    if (list[^1].To < range.From)
    {
        list.Add(range);
        return;
    }

    Console.WriteLine($"what to do? {String.Join(", ", list)} <= {range}");
}

int Distance(Pos sensorPos1, Pos beaconPos1)
{
    return Math.Abs(sensorPos1.X - beaconPos1.X) + Math.Abs(sensorPos1.Y - beaconPos1.Y);
}


Range GetRowOverlapRange(int rowY, Pos sensor, int distance)
{
    int yDist = Math.Abs(sensor.Y - rowY);
    if (yDist > distance)
    {
        return new Range(0, 0);
    }

    int offset = distance - yDist;
    return new Range(sensor.X - offset, sensor.X + offset);
}

(Pos sensorPos, Pos beaconPos, int distance) ParseLine(string line)
{
    int[] numbers = GetNumbers(line).ToArray();

    Pos sensorPos = new(numbers[0], numbers[1]);
    Pos beaconPos = new(numbers[2], numbers[3]);
    return (
        sensorPos,
        beaconPos,
        Distance(sensorPos, beaconPos)
    );
}

IEnumerable<int> GetNumbers(string line)
{
    int currentStart = 0;

    for (int i = 0; i < 4; i++)
    {
        int xStart = line.IndexOf('=', currentStart) + 1;
        int xEnd = line.IndexOf(' ', xStart) - 1;
        if (xEnd < 0)
        {
            xEnd = line.Length;
        }

        if (xStart < 0)
        {
            yield break;
        }

        currentStart = xEnd;

        yield return int.Parse(line.Substring(xStart, xEnd - xStart));
    }
}


public record Pos(int X, int Y);

internal record Range(int From, int To);

internal record Input(string Path, int RowToCheck, int MaxSize);