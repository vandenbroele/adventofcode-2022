Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

Input input = inputs[0];

List<string> allLines = File.ReadLines(input.Path).ToList();


Grid grid = Parse(allLines.SkipLast(2).ToList());
string commandString = allLines.Last();
Character character = new(grid.GetStartPos(), Orientation.Right);

grid.Render(character);


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

internal class Grid
{
    private readonly Tile[,] tiles;

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

    public void Render(Character character)
    {
        for (int row = tiles.GetLength(1) - 1; row >= 0; row--)
        {
            for (int col = 0; col < tiles.GetLength(0); col++)
            {
                if (character.Position.X == col && character.Position.Y == row)
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
}

internal record Character(Position Position, Orientation Orientation)
{
    public void RotateLeft()
    {
    }

    public void RotateRight()
    {
    }
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

internal record Position(int X, int Y);