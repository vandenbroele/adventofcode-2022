Input[] inputs =
{
    new("./testInput.txt"),
    new("./testInput2.txt"),
    new("./input.txt")
};

IList<string> inputLines = File.ReadLines(inputs[2].Path).ToArray();
Valley valley = new(inputLines);

// Get start pos

for (int i = 0; i < 3; i++)
{
    
    valley.Update();
    // Add valid target positions to options until path is reached
    
}
// valley.Render();

internal class Valley
{
    private readonly Blizzard[,] blizzard;
    private readonly Blizzard[,] shadowBlizzard;
    public int Rows => blizzard.GetLength(0);
    public int Columns => blizzard.GetLength(1);

    public Valley(IList<string> input)
    {
        int rowCount = input.Count;
        int columnCount = input[0].Length;

        blizzard = new Blizzard[rowCount, columnCount];

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < columnCount; col++)
            {
                blizzard[row, col] = BlizzardHelpers.Parse(input[row][col]);
            }
        }

        shadowBlizzard = new Blizzard[rowCount, columnCount];
    }

    public void Render()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Console.Write(BlizzardHelpers.ToChar(blizzard[row, col]));
            }

            Console.WriteLine();
        }
    }

    public void Update()
    {
        UpdateShadowBlizzard();

        for (int row = 1; row < Rows - 1; row++)
        {
            for (int col = 1; col < Columns - 1; col++)
            {
                Blizzard val = Blizzard.None;

                // if (shadowBlizzard[RowDown(row), col].Contains(Blizzard.Up))
                // {
                //     val = val.Add(Blizzard.Up);
                // }
                //
                // if (shadowBlizzard[RowUp(row), col].Contains(Blizzard.Down))
                // {
                //     val = val.Add(Blizzard.Down);
                // }

                if (shadowBlizzard[row, ColumnLeft(col)].Contains(Blizzard.Right))
                {
                    val = val.Add(Blizzard.Right);
                }

                if (shadowBlizzard[row, ColumnRight(col)].Contains(Blizzard.Left))
                {
                    val = val.Add(Blizzard.Left);
                }

                blizzard[row, col] = val;
            }
        }
    }

    private int RowUp(int row) => Decrease(row, Rows);

    private int ColumnLeft(int col) => Decrease(col, Columns);

    private static int Decrease(int input, int max)
    {
        int result = input - 1;

        if (result > 1)
            return result;

        // Wrap around
        int smallRowRange = max - 2;
        int smallRow = result - 1;
        int smallResult = (smallRow + smallRowRange) % smallRowRange;
        result = smallResult + 1;

        return result;
    }

    private int RowDown(int row) => Increase(row, Rows);

    private int ColumnRight(int col) => Increase(col, Columns);

    private static int Increase(int input, int max)
    {
        int result = input + 1;

        if (result < max - 1)
            return result;

        // Wrap around
        int smallRowRange = max - 2;
        result = (result - 1) % smallRowRange + 1;

        return result;
    }


    private void UpdateShadowBlizzard()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                shadowBlizzard[row, col] = blizzard[row, col];
            }
        }
    }
}

internal record Input(string Path);

internal record Position(int Col, int Row);

[Flags]
internal enum Blizzard
{
    None = 0x0000,
    Left = 0x0001,
    Up = 0x0010,
    Right = 0x0100,
    Down = 0x1000,
    Blocked = 0x1_0000
}

internal static class BlizzardHelpers
{
    public static bool Contains(this Blizzard blizzard, Blizzard test)
    {
        return (blizzard & test) == test;
    }

    public static Blizzard Add(this Blizzard blizzard, Blizzard other)
    {
        return blizzard | other;
    }

    public static Blizzard Remove(this Blizzard blizzard, Blizzard other)
    {
        return blizzard & ~other;
    }

    public static Blizzard Parse(char input) => input switch
    {
        '#' => Blizzard.Blocked,
        '.' => Blizzard.None,
        '>' => Blizzard.Right,
        '<' => Blizzard.Left,
        'v' => Blizzard.Down,
        '^' => Blizzard.Up,
        _ => throw new ArgumentOutOfRangeException($"what to do with {input}")
    };

    public static char ToChar(Blizzard input)
    {
        return input switch
        {
            Blizzard.None => '.',
            Blizzard.Left => '<',
            Blizzard.Up => '^',
            Blizzard.Right => '>',
            Blizzard.Down => 'v',
            Blizzard.Blocked => '#',
            _ => CountDirections(input)
        };
    }

    private static char CountDirections(Blizzard input)
    {
        int counter = 0;

        if (input.Contains(Blizzard.Up)) ++counter;
        if (input.Contains(Blizzard.Down)) ++counter;
        if (input.Contains(Blizzard.Left)) ++counter;
        if (input.Contains(Blizzard.Right)) ++counter;

        return counter.ToString()[0];
    }
}

internal enum Directions
{
    North,
    East,
    South,
    West
}