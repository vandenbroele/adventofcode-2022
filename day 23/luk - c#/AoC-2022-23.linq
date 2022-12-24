<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	//Run(smallInput, "small sample");
	Run(sampleInput, "sample (110 / 20)");
	Run(puzzleInput, "puzzle (3940 / 990)");
}

public void Run(string input, string name)
{
	var board = new Board(input);
	//board.Print();

	int i = 0;
	for (; i < 10; i++)
	{
		board.CalculateNextLocations(i);
		board.MoveToCalculatedLocations();
	}
	//"-----------------------------".Dump();
	//board.Print();
	board.CountOpenSpaces().Dump(name);

	for (; i < int.MaxValue; i++)
	{
		board.CalculateNextLocations(i);
		if (!board.MoveToCalculatedLocations()) break;
	}
	$"Round: {i + 1}".Dump(name);
}

public class Board
{
	public List<Elf> Elves { get; } = new List<Elf>();

	public Elf this[int x, int y] => this[new Point(x, y)];
	public Elf this[Point p] => Elves.FirstOrDefault(e => e.Location == p);

	public Board(string input)
	{
		var lines = input.Split("\r\n");
		for (int i = 0; i < lines.Length; i++)
		{
			for (int j = 0; j < lines[i].Length; j++)
			{
				if (lines[i][j] != '#') continue;
				Elves.Add(new Elf(this, j, i));
			}
		}
	}

	public void CalculateNextLocations(int iteration) => Elves.AsParallel().ForAll(e => e.CalculateNextLocation(iteration));
	// { foreach (var e in Elves) e.CalculateNextLocation(iteration); }
	public bool MoveToCalculatedLocations() => Elves.AsParallel().Select(e => e.MoveToNextLocation()).ToList().Any(e => e);
	//{
	//	var result = false;
	//	foreach (var e in Elves) result = e.MoveToNextLocation() || result;
	//	return result;
	//}

	public void Print()
	{
		var sb = new StringBuilder("<pre>");

		var minX = Elves.Min(e => e.Location.X);
		var maxX = Elves.Max(e => e.Location.X);
		var minY = Elves.Min(e => e.Location.Y);
		var maxY = Elves.Max(e => e.Location.Y);

		for (int y = minY; y <= maxY; y++)
		{
			for (int x = minX; x <= maxX; x++)
			{
				if (this[x, y] != null) sb.Append('#');
				else sb.Append('.');
			}
			sb.AppendLine();
		}

		Util.RawHtml(sb.ToString()).Dump();
	}

	public int CountOpenSpaces()
	{
		var minX = Elves.Min(e => e.Location.X);
		var maxX = Elves.Max(e => e.Location.X);
		var minY = Elves.Min(e => e.Location.Y);
		var maxY = Elves.Max(e => e.Location.Y);

		var rangeX = Math.Abs(maxX - minX) + 1;
		var rangeY = Math.Abs(maxY - minY) + 1;

		return (rangeX * rangeY) - Elves.Count;
	}
}

public class Elf
{
	public Board Board { get; }
	public Point Location { get; private set; }

	public Point? NextLocation { get; private set; }

	public Elf(Board board, int x, int y)
	{
		Board = board;
		Location = new Point(x, y);
	}

	public bool MoveToNextLocation()
	{
		if (!NextLocation.HasValue)
			return false;
		if (Board.Elves.Where(e => e != this && e.NextLocation.HasValue).Any(e => e.NextLocation.Value == NextLocation.Value))
			return false;

		Location = NextLocation.Value;
		return true;
	}

	public override string ToString() => $"Loc={Location} | Next={NextLocation}";

	public void CalculateNextLocation(int iteration)
	{
		var neighbourLocations = Location.Neighbours();
		var neighbours = neighbourLocations.Select(l => Board[l]).Where(e => e != null).ToArray();
						
		NextLocation = (iteration % 4) switch {		
			_ when !neighbours.Any() => null,
		
			0 when !neighbours.Any(n => n.Location.Y == Location.Y - 1) => Location.N(), // No elves above -> Move N
			0 when !neighbours.Any(n => n.Location.Y == Location.Y + 1) => Location.S(), // No Elves below -> Move S
			0 when !neighbours.Any(n => n.Location.X == Location.X - 1) => Location.W(), // No Elves Left  -> Move W
			0 when !neighbours.Any(n => n.Location.X == Location.X + 1) => Location.E(), // No Elves Right -> Move E

			1 when !neighbours.Any(n => n.Location.Y == Location.Y + 1) => Location.S(), // No Elves below -> Move S
			1 when !neighbours.Any(n => n.Location.X == Location.X - 1) => Location.W(), // No Elves Left  -> Move W
			1 when !neighbours.Any(n => n.Location.X == Location.X + 1) => Location.E(), // No Elves Right -> Move E
			1 when !neighbours.Any(n => n.Location.Y == Location.Y - 1) => Location.N(), // No elves above -> Move N

			2 when !neighbours.Any(n => n.Location.X == Location.X - 1) => Location.W(), // No Elves Left  -> Move W
			2 when !neighbours.Any(n => n.Location.X == Location.X + 1) => Location.E(), // No Elves Right -> Move E
			2 when !neighbours.Any(n => n.Location.Y == Location.Y - 1) => Location.N(), // No elves above -> Move N
			2 when !neighbours.Any(n => n.Location.Y == Location.Y + 1) => Location.S(), // No Elves below -> Move S

			3 when !neighbours.Any(n => n.Location.X == Location.X + 1) => Location.E(), // No Elves Right -> Move E
			3 when !neighbours.Any(n => n.Location.Y == Location.Y - 1) => Location.N(), // No elves above -> Move N
			3 when !neighbours.Any(n => n.Location.Y == Location.Y + 1) => Location.S(), // No Elves below -> Move S
			3 when !neighbours.Any(n => n.Location.X == Location.X - 1) => Location.W(), // No Elves Left  -> Move W

			_ => null
		};
	}
}

public static class PointExtensions
{
	// x-1 x  x+1
	// NW  N  NE  y-1
	// W   #   E  y
	// SW  S  SE  y+1

	public static Point N(this Point p) => new Point(p.X, p.Y - 1);
	public static Point S(this Point p) => new Point(p.X, p.Y + 1);
	
	public static Point E(this Point p) => new Point(p.X + 1, p.Y);
	public static Point W(this Point p) => new Point(p.X - 1, p.Y);

	public static Point NE(this Point p) => new Point(p.X + 1, p.Y - 1);
	public static Point NW(this Point p) => new Point(p.X - 1, p.Y - 1);
	
	public static Point SE(this Point p) => new Point(p.X + 1, p.Y + 1);
	public static Point SW(this Point p) => new Point(p.X - 1, p.Y + 1);


	public static Point[] Top(this Point p) => new[] { p.NW(), p.N(), p.NE() };
	public static Point[] Right(this Point p) => new[] { p.NE(), p.E(), p.SE() };
	public static Point[] Bottom(this Point p) => new[] { p.SW(), p.S(), p.SE() };
	public static Point[] Left(this Point p) => new[] { p.NW(), p.W(), p.SW() };

	public static Point[] Neighbours(this Point p) => new[] { p.N(), p.NE(), p.E(), p.SE(), p.S(), p.SW(), p.W(), p.NW() };
}

public static string smallInput = @".....
..##.
..#..
.....
..##.
.....";

public static string sampleInput = @"....#..
..###.#
#...#.#
.#...##
#.###..
##.#.##
.#..#..";

public static string puzzleInput = @"#.##..##.#..#.##..##....##..#..#####...#.###.#..##..#..#.#...##..##....
##..###.#.#.#..##.##.####.........#...#..#..#.#..#.#....##..###.#.##..#
#.......###....#.#.##.#.#.#.##..###...###..##.....#...#.##.##.......##.
...##.#......#.#.#..#.###..#.###....#....#.###.##....####....##....##..
.#...##..###...##.#.#..#...####.#.##.#.#..#.##.###.###.#.##.##..#.#...#
#..##....##.#..##.##...###..##.#...##...#####...#####.##..#.###....#.#.
###....#....#.##.####.###.#..####..##..##..#..#.##.#######.##....###...
###..###.....###..##..##....#......#.#.##...####.######..###.###....###
#.#.###.###.....###.####.#.####...#.#.###.#...#.##..##.##.#..####..#...
####.##...#....##.##.#.#....##.###....#.#..#.....##...##.#...###.##.#.#
...#.#..####.###...###.#....###...##.####..#.######.##.##..#.#..##...#.
#####......#.####.#####...#...#..#..#.#.#.#.#.#.#.#.##.#..#.###.#....#.
#..#.#.#..#...#.#.##..#.######.##..#.###...#.#.#...#.####..##..##..#.#.
..#.#..###....#.##.##....####.#########..#.########..##....#..#..##..#.
.#.#.#...##...##.#.#..#..#####..###....#..#...####.##.####.###.##..##..
.##.####..#.#######..#.#######..##..###....#.#..#..##.#..#.#..###.....#
#....#.#####..#.##...###.###....#.#.#.#..#.##.####...#...##..#####.##..
...##.###.##...#.##.###.#.###....####.....#.##.##.##.###...###.###.#.##
..###...#...##....#.###.....##..####..#.###.####..##.###....##.#....##.
...#..##..#..##.###.#....###....#..##..#...##.#..##.#.####...##.##..###
######.......####..#..#.#####.......#####..#.#..##..#......#.#.#.#####.
.#...#..#.##.###..#.#.#..####.#.########.#.###..####.#..#.#..###.#####.
...####.#..#..#.##...#.#####.########.##..##....#####.#.##..######.#.##
.....#..##.#.....#..#.#.##..######..####.#..#....##.....###.#.#..##..#.
..##.##..##.#.#.##...##.##...#..##.....#..#..#.#.#.....#.#.##.###...###
##.#..#.#.####.##....#...##.#...#...#..#.##..#...##.....#.#.##.#####...
...#..##..##...#..###....#..#.###..####...#.#..##.###.##......###.#.#.#
..##.#.#.##..###.....#..##.#....#....#....#.##..#.#.####.#....##..#..##
.##..##..#.#.....###..#.#..#.##.#.#.#..#..#.##.#.###...#..#..##.#....##
......#...######....###.##..#.###.####.##.##...#.###...####.##..#..#...
#..##.#.###.##......####...#.#.##.#.#....##.##..#.#.....####...#..###..
..##...#.##..#.....#######.#.#.###.#.##...####..#.#....##.###..##.#.###
...####..#..##.########..#####.##..##...#..##..##..#...###.###....#.###
.###.#..#.##.#..###.#.##.#.#...#.#.#.##..##.#....###.#..#.....#..##..#.
...##.##...#..#.####.....#.##....####.#.###..##...#..#.####...##..##.##
###...##....##..##.#...##......#...##.##.#.....##..##.#..#.##......#..#
.......#.######...#####....#...#####.#####...##..#..#.#.#..........#.#.
#######.#...#.#..#..#..##..#.####..#.##..###..###.##.#...#.#..#.####.##
.#..#......#.#....###.#..##..####.#..#.#..##..#####.#....##.#####.#.#.#
.#.###.###..#..#.#.#.#..##.#......###.##.#....#######.#..#.#####.....##
#.####..###.#..###.#.#...##.##....###.#.#.#.###..#...#...#.##.###...#..
#..##..##.##..##....###..######....#.#####.##..#...#....#.#.###.###....
#.###..##.##..#.....##...#..#..#####.#.##..##..#.#...#.....##.#..#.##.#
##...#...#.##..####..#..#.###....#...#.....#.#.#.###...###...#####.#.##
.#.###.....#.#..#.###########.######..#########.##.##..#.#.#...#..##.##
##.#################.#.##.##....####.....#..#.#.##.###.##..#..#.##..#.#
.##.####.#.#..##.##.#.#..#.###.#..#..#.####....#.###.####.#.#..##..####
##....###.#.####.#.#.............##.....###.##.#.###.####...#####.##.#.
...##.#.#.#####.####.##.#..##.###.###.####.#.#.....#.#.#...#####..####.
######.#....#...#.#.######.#.#....#.##..#..#..#..#.......#..#.##.#.#...
...#..#....#.#.#...###.####..##.#.#..#.#.#.####.#..###...###.######...#
#.#.###......##...#...##....###......#.###...#...###..#..####..##...#.#
#..##.....#........###.##..#.#.#.#.##..#.##.#..###.##.#.####..###.##.#.
.##....#..##.###.#.#.#..#..###..#..#####.##....#..#####.#.##..#.##.###.
.#.##.###.####.#.#..#.###.###.#.......####.##...#..##.#.##.##..#.###...
#.##...#####.#...#.#.##..###.#.##.###########..#...#..#.#.###.#.##..#.#
##...#...#.###.#..##..#...##....#...#.#....###.###.#.#..#...#.#.#.#.###
#.#.######.##.#....#.....#..##...#.#####...##.##...#..###.#...##.#..##.
##.#######.....#....#.###..#..#.###.###..#.....####....#.####..#.....#.
#..#..##..##.#.##......#....#.#.....#......#...##.###..#.#.###.......##
#..#.#..######.#.......##..#...#....#####.#...#.##...#...###..####.##..
.....#.....#.#.##..###.###.##..#.##.##..#.#####..##.##..###..###..#.###
...#.###.#.#.#.##...##.##...#..#..#..####.#.##.#...#.#..##.#.#######.##
.####.####..#.####...#...#.#..##...##.##..#....#.#.###.###..#####...##.
..##.#.###.####.#####...#......#.#.###...###..##.##.###.####.###.#..#.#
#.##..####.#.#..#.##..#.##.#.##.#..###.####.#.#.#.##..#.....##.#.#...#.
.##...#.#.#####.###.#...###.#......#####.....#.#######.##.#..##...###..
##.#.#####......####.####...#####....#..####.#.#.####...####.###.###.##
#.#.#.####..##.##.##.#.##.#.#####.##...#....##..##...#.######.....#..##
.#.....#...###.#.#.###.#..#...###...##.......####...####.###..#....#...
..##.##..#......###...##.....##.#..#...###.##.#.##.##...#.#...#.#..#...";