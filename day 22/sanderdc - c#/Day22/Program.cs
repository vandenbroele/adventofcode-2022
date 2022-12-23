using System.Text;
using System.Drawing;

Input[] inputs =
{
    new("./testInput.txt", 50),
    new("./input.txt", 4)
};

LogGrid.Should = false;

Input input = inputs[1];

List<string> allLines = File.ReadLines(input.Path).ToList();

Grid grid = Parse(allLines.SkipLast(2).ToList());
grid.RegionSize = input.RegionSize;
string commandString = allLines.Last();
Character character = new(grid.GetStartPos(), Orientation.Right);


// grid.Render(character);

// Execute("RR1");
try
{
    Execute(commandString);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    grid.Render(character);
    return;
}

Console.WriteLine();
grid.Render(character);
grid.RenderToImage(character);

int col = character.Position.Col + 1;
int row = grid.Height - character.Position.Row;
int orientation = ScoreOrientation(character.Orientation);
Console.WriteLine($"part 2 : {1000 * row + 4 * col + orientation}");


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
            // if (character.Position.Col == 57
            //     && character.Position.Row == 131
            //     && character.Orientation == Orientation.Left)
            // {
            //     LogGrid.Should = true;
            // }

            if (LogGrid.Should)
            {
                string cmd = numberString.Length > 0
                    ? numberString
                    : charEnumerator.Current.ToString();

                Console.WriteLine($"command: {cmd}");
                grid.Render(character);
            }
            
            // grid.RenderToImage(character);

            if (numberString.Length > 0)
            {
                int moveAmount = int.Parse(numberString);
                (Position newPos, Orientation newOrientation) =
                    grid.Move(character.Position, character.Orientation, moveAmount);
                character.Position = newPos;
                character.Orientation = newOrientation;

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
        (Position newPos, Orientation newOrientation) =
            grid.Move(character.Position, character.Orientation, moveAmount);
        character.Position = newPos;
        character.Orientation = newOrientation;
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

int ScoreOrientation(Orientation o) => o switch
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
    public int RegionSize { get; set; }

    
    // TODO: simplify creating the edges or find a way to validate them
    private readonly List<Edge> edges = new()
    {
        new Edge(
            new Range(new Position(50, 200), new Position(99, 200)),
            new Range(new Position(0, 49), new Position(0, 0)),
            1),
        new Edge(
            new Range(new Position(49, 150), new Position(49, 199)),
            new Range(new Position(0, 99), new Position(0, 50)),
            2),
        new Edge(
            new Range(0, 100, 49, 100),
            new Range(50, 149, 50, 100),
            1),
        new Edge(
            new Range(49, 149, 49, 100),
            new Range(0, 99, 49, 99),
            3),
        new Edge(
            new Range(-1, 99, -1, 50),
            new Range(50, 150, 50, 199),
            2),
        new Edge(
            new Range(100, 149, 149, 149),
            new Range(99, 149, 99, 100),
            1),
        new Edge(
            new Range(100, 149, 100, 100),
            new Range(100, 150, 149, 150),
            3),
        new Edge(
            new Range(150, 150, 150, 199),
            new Range(99, 99, 99, 50),
            2),
        new Edge(
            new Range(100, 99, 100, 50),
            new Range(149, 150, 149, 199),
            2),
        new Edge(
            new Range(-1, 49, -1, 0),
            new Range(50, 199, 99, 199),
            3),
        new Edge(
            new Range(100, 200, 199, 200),
            new Range(49, 0, 0, 0),
            0),
        new Edge(
            new Range(50, 49, 99, 49),
            new Range(49, 49, 49, 0),
            1),
        new Edge(
            new Range(50, 49, 50, 0),
            new Range(50, 50, 99, 50),
            3),
        new Edge(
            new Range(0, -1, 49, -1),
            new Range(100, 199, 149, 199),
            0)
    };

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

    public (Position, Orientation) Move(Position position, Orientation direction, int amount)
    {
        (Position, Orientation) result = (position, direction);
        Position currentPos = position;
        Orientation currentOrientation = direction;

        for (int xOffset = 1; xOffset <= amount; xOffset++)
        {
            (currentPos, currentOrientation) = NextTileInDirection(currentPos, currentOrientation);

            if (LogGrid.Should)
            {
                // Render(new Character(currentPos, currentOrientation));
            }

            Tile nextTile = tiles[currentPos.Col, currentPos.Row];
            if (nextTile == Tile.Solid)
            {
                break;
            }

            result = (currentPos, currentOrientation);
        }

        return result;
    }

    private (Position, Orientation) NextTileInDirection(Position start, Orientation orientation)
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
            (next, orientation) = TransformEdge(next, orientation);
        }

        if (next.Row < 0)
        {
            (next, orientation) = TransformEdge(next, orientation);
        }

        if (next.Col >= Width)
        {
            (next, orientation) = TransformEdge(next, orientation);
        }

        if (next.Col < 0)
        {
            (next, orientation) = TransformEdge(next, orientation);
        }

        // Handle open
        if (tiles[next.Col, next.Row] == Tile.Empty)
        {
            (next, orientation) = TransformEdge(next, orientation);
        }

        return (next, orientation);
    }

    private (Position, Orientation) TransformEdge(Position position, Orientation orientation)
    {
        try
        {
            Edge edge = edges.First(e => e.From.Contains(position));

            int offset = edge.From.GetOffset(position);

            for (int i = 0; i < edge.ClockWiseRotations; i++)
            {
                orientation = orientation.RotateRight();
            }

            Position positionAtOffset = edge.To.PositionAtOffset(offset);

            // if(positionAtOffset.Col < 0 || positionAtOffset.Row<0)

            return (positionAtOffset, orientation);
        }
        catch (Exception)
        {
            Console.WriteLine($"did not find edge for {position} {orientation}");
            throw;
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

    public void Render(Character character) => Console.WriteLine(GetGridString(character));

    public void RenderToImage(Character character)
    {
        string text = GetGridString(character);
        Font font = new Font("Cascadia Mono", 15f);
        Color backColor = Color.Black;
        Color textColor = Color.Beige;
        Color accentColor = Color.Red;

        //first, create a dummy bitmap just to get a graphics object
        Image img = new Bitmap(1, 1);
        Graphics drawing = Graphics.FromImage(img);

        //measure the string to see how big the image needs to be
        SizeF textSize = drawing.MeasureString(text, font);

        //free up the dummy image and old graphics object
        img.Dispose();
        drawing.Dispose();

        //create a new image of the right size
        img = new Bitmap((int)textSize.Width, (int)textSize.Height);

        drawing = Graphics.FromImage(img);

        //paint the background
        drawing.Clear(backColor);

        //create a brush for the text
        Brush textBrush = new SolidBrush(textColor);

        drawing.DrawString(text, font, textBrush, 0, 0);


        string temp = text
            .Replace('.', ' ')
            .Replace('■', ' ');


        Brush accentBrush = new SolidBrush(accentColor);
        drawing.DrawString(temp, font, accentBrush, 0, 0);

        drawing.Save();

        textBrush.Dispose();
        drawing.Dispose();


        Directory.CreateDirectory("./renders");
        img.Save($"./renders/img{++index:0000}.png");
    }

    private static int index = 0;

    private string GetGridString(Character character)
    {
        StringBuilder sb = new();
        sb.AppendLine($"character at {character.Position} {character.Orientation}");
        for (int row = tiles.GetLength(1) - 1; row >= 0; row--)
        {
            for (int col = 0; col < tiles.GetLength(0); col++)
            {
                if (character.Position.Col == col && character.Position.Row == row)
                {
                    sb.Append(character.Orientation switch
                    {
                        Orientation.Up => '^',
                        Orientation.Right => '>',
                        Orientation.Down => 'v',
                        Orientation.Left => '<',
                        _ => '@'
                    });
                    continue;
                }

                sb.Append(tiles[col, row] switch
                {
                    Tile.Open => '.',
                    Tile.Solid => '■',
                    _ => ' '
                });
            }

            sb.AppendLine();
        }

        string gridString = sb.ToString();
        return gridString;
    }
}

public static class LogGrid
{
    public static bool Should;
}

internal record Edge(Range From, Range To, int ClockWiseRotations);

internal class Range
{
    public Position From { get; init; }
    public Position To { get; init; }

    public Range(Position from, Position to)
    {
        From = from;
        To = to;
    }

    public Range(int cFrom, int rFrom, int cTo, int rTo)
    {
        From = new Position(cFrom, rFrom);
        To = new Position(cTo, rTo);
    }

    public bool Contains(Position position)
    {
        if (position.Col == From.Col && position.Col == To.Col)
        {
            return position.Row >= Math.Min(From.Row, To.Row) && position.Row <= Math.Max(From.Row, To.Row);
        }

        if (position.Row == From.Row && position.Row == To.Row)
        {
            return position.Col >= Math.Min(From.Col, To.Col) && position.Col <= Math.Max(From.Col, To.Col);
        }

        return false;
    }

    public int GetOffset(Position position)
    {
        if (position.Col == From.Col)
        {
            return GetOffset(From.Row, To.Row, position.Row);
        }

        if (position.Row == From.Row)
        {
            return GetOffset(From.Col, To.Col, position.Col);
        }

        throw new Exception();
    }

    private static int GetOffset(int from, int to, int pos)
    {
        if (from < to)
        {
            return pos - from;
        }
        else
        {
            return (from - to) - (pos - to);
        }
    }

    public Position PositionAtOffset(int offset)
    {
        if (From.Col == To.Col)
        {
            if (From.Row < To.Row)
            {
                return From with { Row = From.Row + offset };
            }

            return From with { Row = From.Row - offset };
        }

        if (From.Row == To.Row)
        {
            if (From.Col < To.Col)
            {
                return From with { Col = From.Col + offset };
            }

            return From with { Col = From.Col - offset };
        }

        throw new Exception();
    }
}

internal class Character
{
    public Position Position { get; set; }
    public Orientation Orientation { get; set; }

    public Character(Position position, Orientation orientation)
    {
        Position = position;
        Orientation = orientation;
    }

    public void RotateLeft() => Orientation = Orientation.RotateLeft();

    public void RotateRight() => Orientation = Orientation.RotateRight();
}

internal static class OrientationExtensions
{
    public static Orientation RotateLeft(this Orientation orientation) => orientation switch
    {
        Orientation.Left => Orientation.Down,
        Orientation.Down => Orientation.Right,
        Orientation.Right => Orientation.Up,
        Orientation.Up => Orientation.Left,
        _ => Orientation.Right,
    };

    public static Orientation RotateRight(this Orientation orientation) => orientation switch
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

internal record Input(string Path, int RegionSize);

internal record Position(int Col, int Row);