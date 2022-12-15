string[] inputs = { "./input.txt", "./testInput.txt" };
List<string> inputLines = File.ReadLines(inputs[0]).ToList();

// const int rowToCheck = 10;
const int rowToCheck = 2000000;

List<(Pos sensorPos, Pos beaconPos, int distance)> pairs = inputLines.Select(ParseLine).ToList();


HashSet<int> positions = new();

foreach ((Pos sensorPos, Pos _, int distance) in pairs)
{
    (int from, int to) range = GetRowOverlapRange(rowToCheck, sensorPos, distance);

    if (range.to - range.from <= 0)
    {
        continue;
    }

    for (int i = range.from; i <= range.to; i++)
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

Console.WriteLine(positions.Count);


int Distance(Pos sensorPos1, Pos beaconPos1)
{
    return Math.Abs(sensorPos1.X - beaconPos1.X) + Math.Abs(sensorPos1.Y - beaconPos1.Y);
}

(int from, int to) GetRowOverlapRange(int rowY, Pos sensor, int distance)
{
    int yDist = Math.Abs(sensor.Y - rowY);
    if (yDist > distance)
    {
        return (0, 0);
    }

    int offset = distance - yDist;
    return (sensor.X - offset, sensor.X + offset);
}

int GetRowOverlapCount(int rowY, Pos sensor, int distance)
{
    int yDist = Math.Abs(sensor.Y - rowY);
    int maxRow = 1 + 2 * distance;

    return Math.Max(0, maxRow - 2 * yDist);
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