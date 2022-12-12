string[] inputs = { "./input.txt", "./testInput.txt" };

List<string> gridInput = File.ReadLines(inputs[0]).ToList();

char[,] inputGrid = new char[gridInput[0].Length, gridInput.Count];
uint[,] grid = new uint[gridInput[0].Length, gridInput.Count];
Point start = new(0, 0);
Point end = new(gridInput[0].Length - 1, gridInput.Count - 1);

for (int row = 0; row < gridInput.Count; row++)
{
    string line = gridInput[row];
    for (int col = 0; col < line.Length; col++)
    {
        char c = line[col];

        if (c == 'S')
        {
            start = new Point(col, row);
        }

        if (c == 'E')
        {
            end = new Point(col, row);
        }

        grid[col, row] = GetValue(c);
        inputGrid[col, row] = c;
    }
}


Stack<Point> path = new();
path.Push(start);

// bool result = Traverse(path, start);
// Console.WriteLine($"path: {result} ({path.Count})");

uint[,] flooded = GetMaxGrid();
flooded[start.X, start.Y] = 0;
flooded = Flood(flooded, start, IsMoveValidUp);

// DrawGrid(flooded);
Console.WriteLine($"part 1: {flooded[end.X, end.Y]}");

// Part 2 (flood from top)
uint[,] flooded2 = GetMaxGrid();
flooded2[end.X, end.Y] = 0;
flooded2 = Flood(flooded2, end, IsMoveValidDown);

// DrawGrid(flooded2);

uint result = uint.MaxValue;
for (int row = 0; row < grid.GetLength(1); row++)
{
    for (int column = 0; column < grid.GetLength(0); column++)
    {
        char c = inputGrid[column, row];
        if (c is 'a' or 'S')
        {
            result = Math.Min(flooded2[column, row], result);
        }
    }
}
Console.WriteLine($"part 2: {result}");


uint[,] GetMaxGrid()
{
    uint[,] maxGrid = new uint[grid.GetLength(0), grid.GetLength(1)];
    for (int row = 0; row < maxGrid.GetLength(1); row++)
    {
        for (int column = 0; column < maxGrid.GetLength(0); column++)
        {
            maxGrid[column, row] = uint.MaxValue;
        }
    }
    return maxGrid;
}

void DrawGrid(uint[,] uints)
{
    for (int row = 0; row < uints.GetLength(1); row++)
    {
        for (int column = 0; column < uints.GetLength(0); column++)
        {
            // if (end.X == column && end.Y == row)
            // {
            //     Console.Write('E');
            //     continue;
            // }
            //
            // if (start.X == column && start.Y == row)
            // {
            //     Console.Write('S');
            //     continue;
            // }

            uint value = uints[column, row];
            
            Console.Write(value == uint.MaxValue
                ? new string(' ', 4)
                : $"{value: 0000} ");

            // Console.Write(value == uint.MaxValue
            //     ? $".{inputGrid[column, row]}"
            //     : $"X{inputGrid[column, row]}");
        }

        Console.WriteLine();
    }
}

uint[,] Flood(uint[,] input, Point origin, Func<Point, Point, bool> validator)
{
    Queue<Point> priorityQueue = new();
    priorityQueue.Enqueue(origin);

    input[origin.X, origin.Y] = 0;

    while (priorityQueue.Count > 0)
    {
        Point current = priorityQueue.Dequeue();
        uint score = input[current.X, current.Y];
        IEnumerable<Point> neighbours = GetNeighbours(current, validator)
            .Where(n => input[n.X, n.Y] == uint.MaxValue);

        foreach (Point neighbour in neighbours)
        {
            input[neighbour.X, neighbour.Y] = score + 1;
            priorityQueue.Enqueue(neighbour);
        }
    }

    return input;
}

IEnumerable<Point> GetNeighbours(Point point, Func<Point, Point, bool> validator)
{
    Point right = point with { X = point.X + 1 };
    Point left = point with { X = point.X - 1 };
    Point up = point with { Y = point.Y + 1 };
    Point down = point with { Y = point.Y - 1 };

    if (InBounds(right) && validator(point, right)) yield return right;
    if (InBounds(left) && validator(point, left)) yield return left;
    if (InBounds(up) && validator(point, up)) yield return up;
    if (InBounds(down) && validator(point, down)) yield return down;
}

bool IsMoveValidUp(Point from, Point to)
{
    uint fromValue = grid[from.X, from.Y];
    uint toValue = grid[to.X, to.Y];

    return fromValue >= toValue || fromValue + 1 == toValue;
    // return fromValue == toValue || fromValue + 1 == toValue;
}
bool IsMoveValidDown(Point from, Point to)
{
    uint fromValue = grid[from.X, from.Y];
    uint toValue = grid[to.X, to.Y];

    return toValue >= fromValue || toValue + 1 == fromValue;
    // return fromValue == toValue || fromValue + 1 == toValue;
}

bool InBounds(Point point)
{
    return point.X >= 0
        && point.X < grid.GetLength(0)
        && point.Y >= 0
        && point.Y < grid.GetLength(1);
}


uint GetValue(char c) => c switch
{
    'S' => 0,
    'E' => 25,
    _ => (uint)(c - 97)
};

public record Point(int X, int Y);