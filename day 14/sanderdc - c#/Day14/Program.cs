string[] inputs = { "./input.txt", "./testInput.txt" };
List<string> inputLines = File.ReadLines(inputs[0]).ToList();

Grid grid = Grid.ParseLines(inputLines);

bool spawnBlocked = false;
int sandCount = 0;
int sandReachedVoid = 0;
Pos spawnPos = new(500, 0);

while (!spawnBlocked)
{
    if (grid.Blocked(spawnPos))
    {
        spawnBlocked = true;
        break;
    }

    // spawn sand
    Pos sandPos = spawnPos;
    bool resting = false;
    ++sandCount;

    while (!resting)
    {
        // For part 1
        if (grid.InVoid(sandPos) && sandReachedVoid == 0)
        {
            // reachedVoid = true;
            sandReachedVoid = sandCount - 1;
        }

        // Move down
        Pos down = sandPos with { Y = sandPos.Y + 1 };
        if (!grid.Blocked(down))
        {
            sandPos = down;

            continue;
        }

        Pos downLeft = new(sandPos.X - 1, sandPos.Y + 1);
        if (!grid.Blocked(downLeft))
        {
            sandPos = downLeft;
            continue;
        }

        Pos downRight = new(sandPos.X + 1, sandPos.Y + 1);
        if (!grid.Blocked(downRight))
        {
            sandPos = downRight;
            continue;
        }

        // We can no longer move
        grid.AddSand(sandPos);
        resting = true;
    }
}

const int padding = 10;
// grid.Render(494 - padding, 503 + padding);
Console.WriteLine(sandReachedVoid);
Console.WriteLine(sandCount);


public class Grid
{
    private readonly Dictionary<Pos, bool> grid;
    private readonly int voidY;

    private Grid(Dictionary<Pos, bool> grid, int voidY)
    {
        this.grid = grid;
        this.voidY = voidY;
    }

    public void Render(int xMin, int xMax)
    {
        int yMax = voidY + 2;

        for (int row = 0; row < yMax; row++)
        {
            for (int col = xMin; col < xMax + 1; col++)
            {
                Pos pos = new Pos(col, row);
                string p;
                if (grid.ContainsKey(pos))
                {
                    p = grid[pos] ? "#" : "o";
                }
                else
                {
                    p = ".";
                }

                Console.Write(p);
            }

            Console.WriteLine();
        }
    }

    public static Grid ParseLines(List<string> lines)
    {
        int bottom = int.MinValue;
        Dictionary<Pos, bool> grid = new();
        foreach (string line in lines)
        {
            Pos[] points = line.Split(" -> ")
                .Select(ParsePos)
                .ToArray();

            for (int i = 1; i < points.Length; i++)
            {
                foreach (Pos pos in GetLine(points[i - 1], points[i]))
                {
                    if (!grid.ContainsKey(pos))
                    {
                        grid.Add(pos, true);
                    }

                    bottom = Math.Max(bottom, pos.Y + 1);
                }
            }
        }

        return new Grid(grid, bottom);
    }

    private static IEnumerable<Pos> GetLine(Pos start, Pos end)
    {
        if (start.X == end.X)
        {
            int min = Math.Min(start.Y, end.Y);
            int max = Math.Max(start.Y, end.Y);

            for (int i = min; i <= max; ++i)
            {
                yield return start with { Y = i };
            }
        }
        else if (start.Y == end.Y)
        {
            int min = Math.Min(start.X, end.X);
            int max = Math.Max(start.X, end.X);

            for (int i = min; i <= max; ++i)
            {
                yield return start with { X = i };
            }
        }
        else
        {
            throw new Exception();
        }
    }

    private static Pos ParsePos(string input)
    {
        string[] parts = input.Split(',');
        return new Pos(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public bool Blocked(Pos pos)
    {
        return grid.ContainsKey(pos) || pos.Y >= voidY + 1;
    }

    public void AddSand(Pos sandPos)
    {
        if (!grid.ContainsKey(sandPos))
        {
            grid.Add(sandPos, false);
        }
    }

    public bool InVoid(Pos sandPos) => sandPos.Y >= voidY;
}


public record Pos(int X, int Y);