var visited = new HashSet<Position> { new() };

Knot tail = new(new(), new());
Knot head = new(new(), new());

foreach (var instruction in File.ReadAllLines("input.txt"))
{
    var direction = instruction[0];
    var stepCount = int.Parse(instruction[2..]);

    foreach (var _ in Enumerable.Range(0, stepCount))
    {
        head = new Knot(direction switch
        {
            'U' => head.Current with { Y = head.Current.Y - 1 },
            'D' => head.Current with { Y = head.Current.Y + 1 },
            'L' => head.Current with { X = head.Current.X - 1 },
            'R' => head.Current with { X = head.Current.X + 1 },
            _ => head.Current
        }, head.Current);

        if (!head.Current.IsTouching(tail.Current))
        {
            tail = new(head.Previous, tail.Current);
            visited.Add(tail.Current);
        }
    }
}

Console.WriteLine(visited.Count);

public record Position(int Y = 0, int X = 0)
{
    private static readonly (int yDiff, int xDiff)[] NeighbourDiffs = (
        from y in new[] { -1, 0, 1 }
        from x in new[] { -1, 0, 1 }
        select (y, x)).ToArray();

    public bool IsTouching(Position p) => NeighbourDiffs.Any(_ => p.Y == Y + _.yDiff && p.X == X + _.xDiff);
}

public record Knot(Position Current, Position Previous);