Console.WriteLine(Solve(File.ReadAllLines("input.txt"), 1));
Console.WriteLine(Solve(File.ReadAllLines("input.txt"), 9));

int Solve(string[] instructions, int ropeLength)
{
    var visited = new HashSet<Position> { new() };

    var head = new Knot { Current = new(), Previous = new() };
    var knots = new Knot[ropeLength];
    for (var i = 0; i < knots.Length; i++)
    {
        knots[i] = new Knot
        {
            Current = new(), 
            Previous = new(),
            NextKnot = i == 0 ? head : knots[i - 1]
        };
    }

    foreach (var instruction in instructions)
    {
        var direction = instruction[0];
        var stepCount = int.Parse(instruction[2..]);

        foreach (var _ in Enumerable.Range(0, stepCount))
        {
            head.Previous = head.Current;
            head.Current = direction switch
            {
                'U' => head.Current with { Y = head.Current.Y - 1 },
                'D' => head.Current with { Y = head.Current.Y + 1 },
                'L' => head.Current with { X = head.Current.X - 1 },
                'R' => head.Current with { X = head.Current.X + 1 },
                _ => head.Current
            };

            for (var i = 0; i < knots.Length; i++)
            {
                var knot = knots[i];
                if (!knot.NextKnot!.Current.IsTouching(knot.Current))
                {
                    var nextPosition = knot.Current.Diff(knot.NextKnot.Current) switch
                    {
                        (2, 1 or -1) or (2, 0) => knot.NextKnot.Current with { Y = knot.NextKnot.Current.Y + 1 },
                        (-2, 1 or -1) or (-2, 0) => knot.NextKnot.Current with { Y = knot.NextKnot.Current.Y - 1 },
                        (1 or -1, 2) or (0, 2) => knot.NextKnot.Current with { X = knot.NextKnot.Current.X + 1 },
                        (1 or -1, -2) or (0, -2) => knot.NextKnot.Current with { X = knot.NextKnot.Current.X - 1 },
                        _ => knot.NextKnot!.Previous
                    };

                    knot.Previous = knot.Current;
                    knot.Current = nextPosition;
                    if (i == knots.Length - 1)
                    {
                        visited.Add(knot.Current);
                    }
                }
            }
        }
    }

    return visited.Count;
}

public record Position(int Y = 0, int X = 0)
{
    private static readonly (int yDiff, int xDiff)[] NeighbourDiffs = (
        from y in new[] { -1, 0, 1 }
        from x in new[] { -1, 0, 1 }
        select (y, x)).ToArray();

    public bool IsTouching(Position p) => NeighbourDiffs.Any(_ => p.Y == Y + _.yDiff && p.X == X + _.xDiff);
    public (int y, int x) Diff(Position p) => (Y - p.Y, X - p.X);
}

public class Knot
{
    public required Position Current { get; set; }
    public required Position Previous { get; set; }
    public Knot? NextKnot { get; init; }
}