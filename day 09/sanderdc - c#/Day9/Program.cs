// IEnumerable<string> input = File.ReadLines("./testInput1.txt");
// IEnumerable<string> input = File.ReadLines("./testInput2.txt");
IEnumerable<string> input = File.ReadLines("./input.txt");


HashSet<Point> tailTrail = new();

// Point headPos = new(0, 0);
// Point tailPos = new(0, 0);

List<Point> rope = new()
{
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
    new Point(0, 0),
};


tailTrail.Add(new Point(rope[9].X, rope[9].Y));

foreach (string line in input)
{
    string[] command = line.Split(' ');

    Point dir = command[0] switch
    {
        "R" => Point.Right,
        "U" => Point.Up,
        "L" => Point.Left,
        "D" => Point.Down,
        _ => throw new ArgumentOutOfRangeException()
    };

    int moves = int.Parse(command[1]);

    for (int i = 0; i < moves; i++)
    {
        rope[0].Move(dir);

        for (int index = 1; index < rope.Count; index++)
        {
            Follow(rope[index - 1], rope[index]);
        }

        Point tail = rope[^1];
        tailTrail.Add(new Point(tail.X, tail.Y));
    }

    // WriteRopeGrid(rope);
    // WriteTestGrid(rope[0], rope[9], tailTrail);
}

Console.WriteLine(tailTrail.Count);

void WriteRopeGrid(List<Point> points)
{
    const int size = 26;
    const int xOffset = 12;
    const int yOffset = 6;

    for (int row = size - yOffset - 1; row >= -yOffset; row--)
    {
        for (int col = 0 - xOffset; col < size - xOffset; col++)
        {
            Point here = new(col, row);
            Point? point = points.Find(p => p.Equals(here));
            if (point == null)
            {
                Console.Write('.');
                continue;
            }

            int index = points.IndexOf(point);
            Console.Write(index);
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}

void WriteTestGrid(Point head, Point tail, HashSet<Point> hashSet)
{
    const int size = 26;
    const int xOffset = 12;
    const int yOffset = 6;


    for (int row = size - 1; row >= 0; row--)
    {
        for (int col = 0; col < size; col++)
        {
            Point here = new(col, row);

            if (head.Equals(here))
            {
                Console.Write('H');
                continue;
            }

            if (tail.Equals(here))
            {
                Console.Write('T');
                continue;
                ;
            }

            Console.Write(hashSet.Contains(here)
                ? '#'
                : '.');
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}

void Follow(Point leader, Point follower)
{
    int xOffset = leader.X - follower.X;
    int yOffset = leader.Y - follower.Y;

    if (xOffset > 2
        || yOffset > 2)
    {
        throw new Exception("biigger than 2!");
    }

    if (xOffset == 0
        && yOffset == 0)
    {
        // Do nothing
    }
    else if (Math.Abs(xOffset) == 2
             && Math.Abs(yOffset) == 2)
    {
        follower.Move(new Point(xOffset / 2, yOffset / 2));
    }
    else if (Math.Abs(xOffset) == 2)
    {
        follower.Move(Math.Abs(yOffset) == 1
            ? new Point(xOffset / 2, yOffset)
            : new Point(xOffset / 2, 0));
    }
    else if (Math.Abs(yOffset) == 2)
    {
        follower.Move(Math.Abs(xOffset) == 1
            ? new Point(xOffset, yOffset / 2)
            : new Point(0, yOffset / 2));
    }
}


public class Point : IEquatable<Point>
{
    public static readonly Point Right = new(1, 0);

    public static readonly Point Up = new(0, 1);
    public static readonly Point Left = new(-1, 0);
    public static readonly Point Down = new(0, -1);

    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Move(Point dir)
    {
        X += dir.X;
        Y += dir.Y;
    }

    public override bool Equals(object? obj) => ((Point)obj!).Equals(this);

    public bool Equals(Point other) => X == other.X && Y == other.Y;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString()
    {
        return $"{X}:{Y}";
    }
}