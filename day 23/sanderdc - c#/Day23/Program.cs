Input[] inputs =
{
    new("./testInput.txt"),
    new("./testInput2.txt"),
    new("./input.txt")
};

IList<string> inputLines = File.ReadLines(inputs[2].Path).ToArray();

DirectionCandidates directionCandidates = new();
List<Elf> elves = new();

for (int row = 0; row < inputLines.Count; row++)
{
    string line = inputLines[row];
    for (int col = 0; col < line.Length; col++)
    {
        if (line[col] == '#')
        {
            Position position = new(col, row);
            Elf elf = new(directionCandidates, position);
            elves.Add(elf);
        }
    }
}


// RenderGrid();

// Do rounds
int roundCount = 0;
for (;; roundCount++)
{
    Console.WriteLine($"we are at round {roundCount + 1}");
    foreach (Elf elf in elves)
    {
        elf.Propose(HasElfAtPosition);
    }

    Position[] acceptedProposals = elves
        .Where(e => e.Position != e.ProposedPosition)
        .GroupBy(e => e.ProposedPosition)
        .Where(grouping => grouping.Count() == 1)
        .Select(g => g.Key)
        .ToArray();

    if (acceptedProposals.Length <= 0)
    {
        Console.WriteLine("we are done");
        break;
    }


    foreach (Elf elf in elves.Where(e => acceptedProposals.Contains(e.ProposedPosition)))
    {
        elf.AcceptProposal();
    }

    // Part 1
    if (roundCount == 9)
    {
        // Calculate square and score (surface area - elf count)
        (Position min, Position max) = GetBounds();
        int width = max.Col - min.Col + 1;
        int height = max.Row - min.Row + 1;
        int area = width * height;

        Console.WriteLine($"part 1: {area - elves.Count} (at round{roundCount + 1})");
    }

    directionCandidates.NextRound();

    // Console.WriteLine();
    // RenderGrid();
}

Console.WriteLine($"part 2: {roundCount + 1}");


bool HasElfAtPosition(Position position)
{
    return elves.Any(e => e.Position == position);
}

void RenderGrid()
{
    (Position min, Position max) = GetBounds();

    const int padding = 1;
    for (int row = min.Row - padding; row <= max.Row + padding; row++)
    {
        for (int col = min.Col - padding; col <= max.Col + padding; col++)
        {
            Console.Write(HasElfAtPosition(new Position(col, row))
                ? '#'
                : '.');
        }

        Console.WriteLine();
    }
}


(Position min, Position max) GetBounds()
{
    Position min = new(
        elves.Min(e => e.Position.Col),
        elves.Min(e => e.Position.Row)
    );

    Position max = new(
        elves.Max(e => e.Position.Col),
        elves.Max(e => e.Position.Row)
    );

    return (min, max);
}

internal class Elf
{
    public Position Position { get; private set; }
    public Position ProposedPosition { get; private set; }

    private readonly DirectionCandidates directionCandidates;

    public Elf(DirectionCandidates directionCandidates, Position startPosition)
    {
        this.directionCandidates = directionCandidates;
        Position = startPosition;
        ProposedPosition = startPosition;
    }

    public void Propose(Func<Position, bool> hasElfAtPosition)
    {
        // Default case is own position
        ProposedPosition = Position;

        // Get neigbouring tiles and propose a position
        IEnumerable<Position> candidates = GetAroundPositions();

        // Check all clear
        bool hasAnyNeighbours = HasNeighboursIn(hasElfAtPosition, candidates);

        if (!hasAnyNeighbours)
        {
            return;
        }


        // Check ordered directions
        foreach (Directions directionCandidate in directionCandidates.Candidates)
        {
            bool hasNeighbours = HasNeighboursIn(hasElfAtPosition, GetDirectionPositions(directionCandidate));

            if (!hasNeighbours)
            {
                ProposedPosition = directionCandidate switch
                {
                    Directions.North => Position with { Row = Position.Row - 1 },
                    Directions.East => Position with { Col = Position.Col + 1 },
                    Directions.South => Position with { Row = Position.Row + 1 },
                    Directions.West => Position with { Col = Position.Col - 1 },
                    _ => throw new ArgumentOutOfRangeException()
                };

                return;
            }
        }
    }

    private static bool HasNeighboursIn(
        Func<Position, bool> hasElfAtPosition,
        IEnumerable<Position> candidates) => candidates.Any(hasElfAtPosition);

    private IEnumerable<Position> GetAroundPositions()
    {
        yield return new Position(Position.Col - 1, Position.Row - 1);
        yield return new Position(Position.Col - 1, Position.Row);
        yield return new Position(Position.Col - 1, Position.Row + 1);

        yield return new Position(Position.Col, Position.Row + 1);

        yield return new Position(Position.Col + 1, Position.Row + 1);
        yield return new Position(Position.Col + 1, Position.Row);
        yield return new Position(Position.Col + 1, Position.Row - 1);

        yield return new Position(Position.Col, Position.Row - 1);
    }

    private IEnumerable<Position> GetDirectionPositions(Directions direction)
    {
        switch (direction)
        {
            case Directions.West:
                yield return new Position(Position.Col - 1, Position.Row - 1);
                yield return new Position(Position.Col - 1, Position.Row);
                yield return new Position(Position.Col - 1, Position.Row + 1);
                break;
            case Directions.South:
                yield return new Position(Position.Col - 1, Position.Row + 1);
                yield return new Position(Position.Col, Position.Row + 1);
                yield return new Position(Position.Col + 1, Position.Row + 1);
                break;
            case Directions.East:
                yield return new Position(Position.Col + 1, Position.Row - 1);
                yield return new Position(Position.Col + 1, Position.Row);
                yield return new Position(Position.Col + 1, Position.Row + 1);
                break;
            case Directions.North:
                yield return new Position(Position.Col - 1, Position.Row - 1);
                yield return new Position(Position.Col, Position.Row - 1);
                yield return new Position(Position.Col + 1, Position.Row - 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public void AcceptProposal()
    {
        Position = ProposedPosition;
    }
}

internal class DirectionCandidates
{
    public IReadOnlyList<Directions> Candidates => directionCandidates;
    private readonly List<Directions> directionCandidates;

    public DirectionCandidates()
    {
        directionCandidates = new List<Directions>
        {
            Directions.North, Directions.South, Directions.West, Directions.East
        };
    }

    public void NextRound()
    {
        Directions move = directionCandidates[0];
        directionCandidates.RemoveAt(0);
        directionCandidates.Add(move);
    }
}

internal record Input(string Path);

internal record Position(int Col, int Row);

enum Directions
{
    North,
    East,
    South,
    West
}