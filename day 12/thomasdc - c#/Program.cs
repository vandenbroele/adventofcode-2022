var (heights, start, end) = ParseInput("input.txt");

var height = heights.Length;
var width = heights[0].Length;

(int x, int y)[] neighbourVectors = {(0, -1), (0, 1), (-1, 0), (1, 0)};
IEnumerable<Position> ValidNeighbours(Position p) =>
    from v in neighbourVectors
    let pos = new Position(p.Y + v.y, p.X + v.x)
    where !(pos.X < 0 || pos.X >= width || pos.Y < 0 || pos.Y >= height)
    where heights[pos.Y][pos.X] - heights[p.Y][p.X] < 2
    select pos;

Console.WriteLine(ShortestPath(start, end));

Console.WriteLine(heights
    .Select((column, y) => column.Select((h, x) => (y, x, h)))
    .SelectMany(_ => _)
    .Where(_ => _.h == 0)
    .Select(_ => ShortestPath(new Position(_.y, _.x), end))
    .Min());

int ShortestPath(Position source, Position target)
{
    var dist = new Dictionary<Position, int>
    {
        [source] = 0
    };

    var prev = new Dictionary<Position, Position>();
    var visited = new HashSet<Position>();
    var priorityQ = new PriorityQueue<Position, int>();
    priorityQ.Enqueue(source, dist[source]);

    // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm#Using_a_priority_queue
    while (priorityQ.TryDequeue(out var current, out _))
    {
        visited.Add(current);
        var validNeighbours = ValidNeighbours(current).ToArray();
        foreach (var neighbour in validNeighbours)
        {
            var alt = dist[current] + 1;
            if (neighbour == target)
            {
                return alt;
            }

            if (alt < dist.GetValueOrDefault(neighbour, int.MaxValue))
            {
                dist[neighbour] = alt;
                prev[neighbour] = current;
                if (!visited.Contains(neighbour))
                {
                    priorityQ.Enqueue(neighbour, alt);
                }
            }
        }
    }

    return int.MaxValue;
}

static (int[][] heights, Position start, Position end) ParseInput(string fileName)
{
    var grid = File.ReadAllLines(fileName);
    var height = grid.Length;
    var heights = new int[grid.Length][];
    Position start = new(0, 0);
    Position end = new(0, 0);
    for (var y = 0; y < height; y++)
    {
        var width = grid[y].Length;
        heights[y] = new int[width];
        for (var x = 0; x < width; x++)
        {
            heights[y][x] = grid[y][x] switch
            {
                'S' => 'a',
                'E' => 'z',
                var c => (int)c
            } - 'a';

            if (grid[y][x] == 'S')
            {
                start = new(y, x);
            }

            if (grid[y][x] == 'E')
            {
                end = new(y, x);
            }
        }
    }

    return (heights, start, end);
}

record Position(int Y, int X);