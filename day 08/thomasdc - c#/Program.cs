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

var scenicScores = new Dictionary<(int y, int x), int>();
for (var y = 0; y < height; y++)
{
    for (var x = 0; x < width; x++)
    {
        var distanceTop = 0;
        for (var yy = y - 1; yy >= 0; yy--)
        {
            distanceTop++;
            if (grid[yy][x] >= grid[y][x])
            {
                break;
            }
        }

        var distanceBottom = 0;
        for (var yy = y + 1; yy < height; yy++)
        {
            distanceBottom++;
            if (grid[yy][x] >= grid[y][x])
            {
                break;
            }
        }

        var distanceLeft = 0;
        for (var xx = x - 1; xx >= 0; xx--)
        {
            distanceLeft++;
            if (grid[y][xx] >= grid[y][x])
            {
                break;
            }
        }

        var distanceRight = 0;
        for (var xx = x + 1; xx < width; xx++)
        {
            distanceRight++;
            if (grid[y][xx] >= grid[y][x])
            {
                break;
            }
        }

        scenicScores.Add((y, x), distanceTop * distanceBottom * distanceLeft * distanceRight);
    }
}

Console.WriteLine(scenicScores.Max(_ => _.Value));
