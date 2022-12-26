using System.Collections;

Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

Input input = inputs[1];
Jet jets = new(File.ReadAllText(input.Path));
ShapeGenerator shapeGenerator = new();
Grid grid = new();


HashSet<(int, int, int)> repetitions = new();
int index = 0;
bool foundRep = false;
int repIndex = -1;

// Code below with logging gave me data for some magic iteration numbers
grid.BlockPlaced += () =>
{
    ++index;

    if (foundRep)
    {
        if (index % repIndex == 0)
        {
            // Console.WriteLine($"({index}) {shapeGenerator.Index} {jets.Index} {grid.CurrentMaxY}");
        }

        return;
    }

    if (!repetitions.Add((shapeGenerator.Index, jets.Index, grid.CurrentMaxY)))
    {
        if(index < 5000)
            return;

        // Console.WriteLine($"stuff at index {index}");
        foundRep = true;
        repIndex = index;
    }
};


Shape? currentShape = null;
const long dropCount = 1000000000000;
const int dropCountPart1 = 2022;

long bottomAt1 = 0;

for (long fallenRocks = 0; fallenRocks < dropCount;)
{

    // Did some notepad calculations to get the following magic numbers
    if (fallenRocks == 10000)
    {
        bottomAt1 = grid.Bottom;
    }
    if (fallenRocks == 225000)
    {
        //bottomOffset should be delta height between iterations
        var bottomOffset = grid.Bottom-bottomAt1;
        while (fallenRocks< dropCount)
        {
            grid.Bottom += bottomOffset;
            fallenRocks += 215000;
        }

        grid.Bottom -= bottomOffset;
        fallenRocks -= 215000;
    }
    
    
    if (currentShape == null)
    {
        // Spawn
        shapeGenerator.MoveNext();
        currentShape = shapeGenerator.Current;

        currentShape.Position = new Position(2, grid.CurrentMaxY + 3);

        // if (fallenRocks < 11)
        // {
        //     Console.WriteLine("spawn");
        //     grid.RenderWithShape(currentShape);
        // }
    }
    
    // Jet
    jets.MoveNext();
    Direction dir = jets.Current;

    switch (dir)
    {
        case Direction.Left:
            if (currentShape.Position.X > 0 && grid.CanGoLeft(currentShape))
            {
                currentShape.Position = currentShape.Position with { X = currentShape.Position.X - 1 };
            }

            break;
        case Direction.Right:
            if (currentShape.MaxX < 6 && grid.CanGoRight(currentShape))
            {
                currentShape.Position = currentShape.Position with { X = currentShape.Position.X + 1 };
            }

            break;
    }


    // Console.WriteLine("push");
    // grid.RenderWithShape(currentShape);

    // Drop
    currentShape.Position = currentShape.Position with { Y = currentShape.Position.Y - 1 };


    // Console.WriteLine("drop");
    // grid.RenderWithShape(currentShape);


    // Collision
    if ( /*currentShape.Position.Y <= grid.CurrentMaxY + 1 &&*/ grid.Collides(currentShape))
    {
        // Move back up one
        currentShape.Position = currentShape.Position with { Y = currentShape.Position.Y + 1 };

        // Console.WriteLine("placing");
        // grid.RenderWithShape(currentShape);


        grid.PlaceShape(currentShape);
        currentShape = null;
        ++fallenRocks;

        if (dropCountPart1 == fallenRocks)
        {
            Console.WriteLine($"part1: {grid.CurrentMaxY + grid.Bottom}");
        }
    }
}

Console.WriteLine($"part2: {grid.CurrentMaxY + grid.Bottom}");

internal class Grid
{
    private readonly List<Position> solidPositions = new()
    {
        new Position(0, -1),
        new Position(1, -1),
        new Position(2, -1),
        new Position(3, -1),
        new Position(4, -1),
        new Position(5, -1),
        new Position(6, -1),
    };

    public event Action BlockPlaced;

    public int CurrentMaxY { get; private set; }
    public long Bottom { get; set; }

    private int[] lowestPositions = { -1, -1, -1, -1, -1, -1, -1 };
    private int currentBottom = -1;


    public void PlaceShape(Shape shape)
    {
        Position[] collection = shape.GetPositions().ToArray();
        solidPositions.AddRange(collection);
        CurrentMaxY = Math.Max(CurrentMaxY, collection.Max(p => p.Y) + 1);

        for (int i = 0; i < 7; i++)
        {
            int lowestPosition = Math.Max(lowestPositions[i], solidPositions.Where(p => p.X == i).Max(p => p.Y));
            lowestPositions[i] = lowestPosition;
        }

        // Console.WriteLine($"floor: {string.Join(",", lowestPositions)}");

        int floor = lowestPositions.Min();
        // Console.WriteLine(floor);
        if (floor > 0)
        {
            ClearBottom(floor - 1);
        }

        BlockPlaced?.Invoke();
    }

    private void ClearBottom(int floor)
    {
        CurrentMaxY -= floor;
        Bottom += floor;

        for (int i = 0; i < lowestPositions.Length; i++)
        {
            lowestPositions[i] -= floor;
        }

        solidPositions.RemoveAll(p => p.Y < floor);

        for (int i = 0; i < solidPositions.Count; i++)
        {
            solidPositions[i] = solidPositions[i] with { Y = solidPositions[i].Y - floor };
        }

        // Call this after each drop

        // Find lowest items that span entire width

        // move all items down by the gap and toss all items below the gap
        // This should keep things inside int range

        // we might also need to offset the 'current shape' outside of this scope
        // Maybe return an offset here. We call clear after each drop
    }

    public bool Collides(Shape shape)
    {
        foreach (Position position in shape.GetPositions())
        {
            if (solidPositions.Contains(position))
            {
                return true;
            }
        }

        return false;
    }


    public bool CanGoLeft(Shape currentShape)
    {
        Position originalPosition = currentShape.Position;
        currentShape.Position = currentShape.Position with { X = currentShape.Position.X - 1 };

        bool result = !Collides(currentShape);

        currentShape.Position = originalPosition;
        return result;
    }

    public bool CanGoRight(Shape currentShape)
    {
        Position originalPosition = currentShape.Position;
        currentShape.Position = currentShape.Position with { X = currentShape.Position.X + 1 };

        bool result = !Collides(currentShape);

        currentShape.Position = originalPosition;
        return result;
    }

    public void Render() => RenderWithShape(null);


    public void RenderWithShape(Shape? currentShape)
    {
        int maxY = CurrentMaxY;
        List<Position> shapeBlocks;
        if (currentShape != null)
        {
            shapeBlocks = currentShape.GetPositions().ToList();
            maxY = shapeBlocks.Max(p => p.Y);
        }
        else
        {
            shapeBlocks = new List<Position>();
        }

        Console.WriteLine();
        for (int y = maxY; y >= 0; y--)
        {
            Console.Write('|');
            for (int x = 0; x < 7; x++)
            {
                Position position = new(x, y);
                if (shapeBlocks.Contains(position))
                {
                    Console.Write('@');
                }
                else
                {
                    Console.Write(solidPositions.Contains(position)
                        ? '#'
                        : '.');
                }
            }

            Console.Write('|');
            Console.WriteLine();
        }


        Console.WriteLine("+-------+");
    }
}

internal class Shape
{
    public static IEnumerator<Shape> Spawner()
    {
        while (true)
        {
            // Horizontal line
            yield return new Shape(new[]
            {
                new Position(0, 0),
                new Position(1, 0),
                new Position(2, 0),
                new Position(3, 0)
            });

            // Cross
            yield return new Shape(new[]
            {
                new Position(1, 0),
                new Position(0, 1),
                new Position(1, 1),
                new Position(2, 1),
                new Position(1, 2)
            });


            // Inverse L
            yield return new Shape(new[]
            {
                new Position(0, 0),
                new Position(1, 0),
                new Position(2, 0),
                new Position(2, 1),
                new Position(2, 2)
            });

            // Vertical line
            yield return new Shape(new[]
            {
                new Position(0, 0),
                new Position(0, 1),
                new Position(0, 2),
                new Position(0, 3)
            });

            // Block
            yield return new Shape(new[]
            {
                new Position(0, 0),
                new Position(1, 0),
                new Position(0, 1),
                new Position(1, 1)
            });
        }
    }

    public Position Position { get; set; } = new(0, 0);
    public int MaxX => Position.X + maxX;

    // relative to bottom left
    private readonly Position[] blocks;
    private readonly int maxX;

    public Shape(Position[] blocks)
    {
        this.blocks = blocks;
        maxX = blocks.Max(p => p.X);
    }

    public IEnumerable<Position> GetPositions() => blocks
        .Select(position => new Position(Position.X + position.X, Position.Y + position.Y));
}

internal enum Direction
{
    Left,
    Right,
    Unknown
}

internal class ShapeGenerator : IEnumerator<Shape>
{
    public int Index { get; private set; } = -1;

    private readonly List<Shape> shapes = new()
    {
        // Horizontal line
        new Shape(new[]
        {
            new Position(0, 0),
            new Position(1, 0),
            new Position(2, 0),
            new Position(3, 0)
        }),

        // Cross
        new Shape(new[]
        {
            new Position(1, 0),
            new Position(0, 1),
            new Position(1, 1),
            new Position(2, 1),
            new Position(1, 2)
        }),


        // Inverse L
        new Shape(new[]
        {
            new Position(0, 0),
            new Position(1, 0),
            new Position(2, 0),
            new Position(2, 1),
            new Position(2, 2)
        }),

        // Vertical line
        new Shape(new[]
        {
            new Position(0, 0),
            new Position(0, 1),
            new Position(0, 2),
            new Position(0, 3)
        }),

        // Block
        new Shape(new[]
        {
            new Position(0, 0),
            new Position(1, 0),
            new Position(0, 1),
            new Position(1, 1)
        }),
    };

    public bool MoveNext()
    {
        Index = (Index + 1) % shapes.Count;
        Current = shapes[Index];

        return true;
    }

    public void Reset()
    {
        Index = -1;
    }

    public Shape Current { get; private set; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
    }
}

internal class Jet : IEnumerator<Direction>
{
    private readonly string input;

    public int Index { get; private set; } = -1;

    public Jet(string input)
    {
        this.input = input;
    }

    public bool MoveNext()
    {
        Index = (Index + 1) % input.Length;
        Current = input[Index] switch
        {
            '<' => Direction.Left,
            '>' => Direction.Right,
            _ => Direction.Unknown
        };

        if (Current == Direction.Unknown)
        {
            MoveNext();
        }


        return true;
    }

    public void Reset()
    {
        Index = -1;
    }

    public Direction Current { get; private set; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
    }
}

internal record Input(string Path);

internal record Position(int X, int Y);