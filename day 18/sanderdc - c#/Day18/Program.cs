Input[] inputs =
{
    new("./testInput.txt"),
    new("./testInput2.txt"),
    new("./input.txt")
};

List<Position> positions = File.ReadAllLines(inputs[2].Path)
    .Select(line => line.Split(','))
    .Select(parts => new Position(
        int.Parse(parts[0]),
        int.Parse(parts[1]),
        int.Parse(parts[2])))
    .ToList();


int counter = 0;
foreach (Position position in positions)
{
    foreach (Position neighbouringPosition in GetNeighbouringPositions(position))
    {
        if (!positions.Contains(neighbouringPosition))
        {
            counter++;
        }
    }
}

Console.WriteLine($"part 1: {counter}");

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