Input[] inputs =
{
    new("./testInput.txt"),
    new("./testInput2.txt"),
    new("./input.txt")
};

Dictionary<Position, bool> bubbleMap = new();
List<Position> positions = File.ReadAllLines(inputs[2].Path)
    .Select(line => line.Split(','))
    .Select(parts => new Position(
        int.Parse(parts[0]),
        int.Parse(parts[1]),
        int.Parse(parts[2])))
    .ToList();


int counter1 = 0;
foreach (Position position in positions)
{
    foreach (Position neighbouringPosition in GetNeighbouringPositions(position))
    {
        if (!positions.Contains(neighbouringPosition))
        {
            counter1++;
        }
    }
}

Console.WriteLine($"part 1: {counter1}");

// Find all interior bubbles

Position min = new Position(
    positions.Min(p => p.X - 1),
    positions.Min(p => p.Y - 1),
    positions.Min(p => p.Z - 1));

Position max = new Position(
    positions.Max(p => p.X + 1),
    positions.Max(p => p.Y + 1),
    positions.Max(p => p.Z + 1));


int counter2 = 0;
foreach (Position position in positions)
{
    foreach (Position neighbouringPosition in GetNeighbouringPositions(position))
    {
        if (!positions.Contains(neighbouringPosition))
        {
            if (!IsBubble(neighbouringPosition, positions))
            {
                counter2++;
            }
        }
    }
}

Console.WriteLine($"part 2: {counter2}");


bool IsBubble(Position airPos, ICollection<Position> solidPositions)
{
    // If in bubbles list: it is a bubble
    if (bubbleMap.ContainsKey(airPos))
        return bubbleMap[airPos];


    // See if we can pathfind to min or max

    List<Position> checkedPositions = new();
    Stack<Position> candidates = new();
    candidates.Push(airPos);

    bool isBubble = true;

    while (candidates.Count > 0)
    {
        Position c = candidates.Pop();

        bool reachedEdge = c.X <= min.X || c.Y <= min.Y || c.Z <= min.Z
            || c.X >= max.X || c.Y >= max.Y || c.Z >= max.Z;

        if (reachedEdge)
        {
            isBubble = false;
            break;
        }

        checkedPositions.Add(c);

        foreach (Position neighbour in GetNeighbouringPositions(c)
                     .Where(n => !checkedPositions.Contains(n))
                     .Where(n => !solidPositions.Contains(n)))
        {
            if (bubbleMap.ContainsKey(neighbour))
            {
                bool isNeighbourBubble = bubbleMap[neighbour];
                if (isNeighbourBubble)
                {
                    Console.WriteLine("something went wrong :/");
                    break;
                }

                isBubble = false;
                break;
            }

            candidates.Push(neighbour);
        }
    }


    // Add to memo
    if (isBubble)
    {
        foreach (Position checkedPosition in checkedPositions)
        {
            bubbleMap[checkedPosition] = true;
        }
    }
    else
    {
        foreach (Position checkedPosition in checkedPositions)
        {
            bubbleMap[checkedPosition] = false;
        }
    }

    return isBubble;
}


IEnumerable<Position> GetNeighbouringPositions(Position position)
{
    yield return position with { X = position.X - 1 };
    yield return position with { X = position.X + 1 };

    yield return position with { Y = position.Y - 1 };
    yield return position with { Y = position.Y + 1 };

    yield return position with { Z = position.Z - 1 };
    yield return position with { Z = position.Z + 1 };
}

internal record Input(string Path);

internal record Position(int X, int Y, int Z);