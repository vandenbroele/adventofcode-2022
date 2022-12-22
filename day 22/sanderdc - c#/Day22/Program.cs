Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

Input input = inputs[1];

List<string> allLines = File.ReadLines(input.Path).ToList();


Grid grid = Parse(allLines.SkipLast(2).ToList());
string commandString = allLines.Last();
Character character = new(grid.GetStartPos(), Orientation.Right);

grid.Render(character);

Execute(commandString);

Console.WriteLine();
grid.Render(character);

int col = character.Position.Col + 1;
int row = grid.Height - character.Position.Row;
int orientation = ScoreOrientation(character.Orientation);
Console.WriteLine($"part 1 : {1000 * row + 4 * col + orientation}");


void Execute(string commands)
{
    using CharEnumerator charEnumerator = commands.GetEnumerator();
    string numberString = "";
    while (charEnumerator.MoveNext())
    {
        if (char.IsNumber(charEnumerator.Current))
        {
            numberString += charEnumerator.Current;
        }

        else
        {
            if (numberString.Length > 0)
            {
                int moveAmount = int.Parse(numberString);
                Position newPos = grid.Move(character.Position, character.Orientation, moveAmount);
                character.Position = newPos;

                numberString = "";
            }

            switch (charEnumerator.Current)
            {
                case 'L':
                    character.RotateLeft();
                    break;
                case 'R':
                    character.RotateRight();
                    break;
            }
        }
    }

    if (numberString.Length > 0)
    {
        int moveAmount = int.Parse(numberString);
        Position newPos = grid.Move(character.Position, character.Orientation, moveAmount);
        character.Position = newPos;
    }
}

Grid Parse(IList<string> gridLines)
{
    using IEnumerator<string> lines = gridLines.GetEnumerator();
    int width = gridLines.Max(l => l.Length);
    int height = gridLines.Count;
    List<(Position, Tile)> positions = new(width * height);

    for (int row = 0; row < gridLines.Count; row++)
    {
        for (int col = 0; col < gridLines[row].Length; col++)
        {
            positions.Add((new Position(col, gridLines.Count - 1 - row), gridLines[row][col] switch
            {
                '.' => Tile.Open,
                '#' => Tile.Solid,
                _ => Tile.Empty
            }));
        }
    }


    return new Grid(width, height, positions);
}

int ScoreOrientation(Orientation orientation) => orientation switch
{
    Orientation.Right => 0,
    Orientation.Down => 1,
    Orientation.Left => 2,
    Orientation.Up => 3,
    _ => throw new ArgumentOutOfRangeException()
};

internal class Grid
{
    private readonly Tile[,] tiles;

    public int Width => tiles.GetLength(0);
    public int Height => tiles.GetLength(1);

    public Grid(int width, int height, List<(Position, Tile)> positions)
    {
        tiles = new Tile[width, height];

        for (int col = 0; col < tiles.GetLength(0); col++)
        {
            for (int row = 0; row < tiles.GetLength(1); row++)
            {
                tiles[col, row] = Tile.Empty;
            }
        }

        foreach (((int col, int row), Tile tile) in positions)
        {
            tiles[col, row] = tile;
        }
    }

    public Position Move(Position position, Orientation direction, int amount)
    {
        Position result = position;
        Position currentPos = position;

        for (int xOffset = 1; xOffset <= amount; xOffset++)
        {
            currentPos = NextTileInDirection(currentPos, direction);

            Tile nextTile = tiles[currentPos.Col, currentPos.Row];
            if (nextTile == Tile.Solid)
            {
                break;
            }

            result = currentPos;
        }

        return result;
    }

    private Position NextTileInDirection(Position start, Orientation orientation)
    {
        Position next = orientation switch
        {
            Orientation.Right => start with { Col = start.Col + 1 },
            Orientation.Left => start with { Col = start.Col - 1 },
            Orientation.Up => start with { Row = start.Row + 1 },
            Orientation.Down => start with { Row = start.Row - 1 },
            _ => start
        };

        if (next.Row >= Height)
        {
            next = next with { Row = 0 };
        }

        if (next.Row < 0)
        {
            next = next with { Row = Height - 1 };
        }

        if (next.Col >= Width)
        {
            next = next with { Col = 0 };
        }

        if (next.Col < 0)
        {
            next = next with { Col = Width - 1 };
        }

        // Handle open
        if (tiles[next.Col, next.Row] == Tile.Empty)
        {
            next = NextTileInDirection(next, orientation);
        }

        return next;
    }

    public Position GetStartPos()
    {
        int row = tiles.GetLength(1) - 1;
        for (int col = 0; col < tiles.GetLength(0); col++)
        {
            if (tiles[col, row] == Tile.Open)
            {
                return new Position(col, row);
            }
        }

        throw new Exception("No start pos found");
    }

    public void Render(Character character)
    {
        for (int row = tiles.GetLength(1) - 1; row >= 0; row--)
        {
            for (int col = 0; col < tiles.GetLength(0); col++)
            {
                if (character.Position.Col == col && character.Position.Row == row)
                {
                    Console.Write(character.Orientation switch
                    {
                        Orientation.Up => '^',
                        Orientation.Right => '>',
                        Orientation.Down => 'v',
                        Orientation.Left => '<',
                        _ => '@'
                    });
                    continue;
                }

                Console.Write(tiles[col, row] switch
                {
                    Tile.Open => '.',
                    Tile.Solid => '#',
                    _ => ' '
                });
            }

            Console.WriteLine();
        }
    }
}

internal class Character
{
    public Position Position { get; set; }
    public Orientation Orientation { get; private set; }

    public Character(Position position, Orientation orientation)
    {
        Position = position;
        Orientation = orientation;
    }

    public void RotateLeft() => Orientation = Orientation switch
    {
        Orientation.Left => Orientation.Down,
        Orientation.Down => Orientation.Right,
        Orientation.Right => Orientation.Up,
        Orientation.Up => Orientation.Left,
        _ => Orientation.Right,
    };

    public void RotateRight() => Orientation = Orientation switch
    {
        Orientation.Left => Orientation.Up,
        Orientation.Up => Orientation.Right,
        Orientation.Right => Orientation.Down,
        Orientation.Down => Orientation.Left,
        _ => Orientation.Right,
    };
}

internal enum Tile
{
    Empty,
    Open,
    Solid
}

internal enum Orientation
{
    Up,
    Right,
    Down,
    Left
}

internal record Input(string Path);

internal record Position(int Col, int Row);