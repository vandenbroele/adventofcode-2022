var grid = File.ReadAllLines("input.txt")
    .Select(line => line.Select(cell => int.Parse(cell.ToString())).ToArray()).ToArray();

var height = grid.Length;
var width = grid[0].Length;

var visible = new HashSet<(int y, int x)>();

// From the top
for (var x = 0; x < width; x++)
{
    var max = int.MinValue;
    for (var y = 0; y < height; y++)
    {
        if (grid[y][x] > max)
        {
            visible.Add((y, x));
            max = grid[y][x];
        }
    }
}

// From the bottom
for (var x = 0; x < width; x++)
{
    var max = int.MinValue;
    for (var y = height - 1; y >= 0; y--)
    {
        if (grid[y][x] > max)
        {
            visible.Add((y, x));
            max = grid[y][x];
        }
    }
}

// From the left
for (var y = 0; y < height; y++)
{
    var max = int.MinValue;
    for (var x = 0; x < width; x++)
    {
        if (grid[y][x] > max)
        {
            visible.Add((y, x));
            max = grid[y][x];
        }
    }
}

// From the right
for (var y = 0; y < height; y++)
{
    var max = int.MinValue;
    for (var x = width - 1; x >= 0; x--)
    {
        if (grid[y][x] > max)
        {
            visible.Add((y, x));
            max = grid[y][x];
        }
    }
}

Console.WriteLine(visible.Count);