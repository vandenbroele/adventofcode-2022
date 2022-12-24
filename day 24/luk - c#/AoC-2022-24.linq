<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	//Run(smallInput, "small");
	Run(sampleInput, "sample");
	Run(puzzleInput, "puzzle");
}

public void Run(string input, string name)
{
	var cavern = new Cavern(input);
	//cavern.Print();
	cavern.StartAtStart();
	
	var minute = 1;
	
	for (; minute < int.MaxValue; minute++)
	{
		cavern.MoveBlizards();
		cavern.AddPossiblePositions();
		//cavern.Print();
		if (cavern.PossiblePositions.Contains(cavern.EndPosition))
			break;
	}	
	minute.Dump(name + " (Part 1)");
	//cavern.Print();

	cavern.StartAtEnd();
	for (; minute < int.MaxValue; minute++)
	{
		cavern.MoveBlizards();
		cavern.AddPossiblePositions();
		//cavern.Print();
		if (cavern.PossiblePositions.Contains(cavern.StartPosition))
			break;
	}

	cavern.StartAtStart();
	for (; minute < int.MaxValue; minute++)
	{
		cavern.MoveBlizards();
		cavern.AddPossiblePositions();
		//cavern.Print();
		if (cavern.PossiblePositions.Contains(cavern.EndPosition))
			break;
	}
	(minute + 2).Dump(name + " (Part 2)");
}

public class Cavern
{
	public List<Point> Walls { get; } = new List<Point>();
	public List<Blizzard> Blizzards { get; } = new List<Blizzard>();

	public Point StartPosition => new Point(1, 0);
	public Point EndPosition => new Point(MaxX - 1, MaxY);
	public List<Point> PossiblePositions { get; private set; }

	public int MinX { get; }
	public int MaxX { get; }
	public int MinY { get; }
	public int MaxY { get; }

	public Cavern(string input)
	{
		var lines = input.Split("\r\n");
		for (int y = 0; y < lines.Length; y++)
		{
			for (int x = 0; x < lines[y].Length; x++)
			{
				if (lines[y][x] == '.') continue;
				else if (lines[y][x] == '#') Walls.Add(new Point(x, y));
				else Blizzards.Add(new Blizzard(this, x, y, lines[y][x]));
			}
		}

		MinX = Walls.Min(e => e.X);
		MaxX = Walls.Max(e => e.X);
		MinY = Walls.Min(e => e.Y);
		MaxY = Walls.Max(e => e.Y);

		PossiblePositions = new List<Point> { StartPosition };
	}

	public void StartAtStart() => PossiblePositions = new List<Point> { StartPosition };
	public void StartAtEnd() => PossiblePositions = new List<Point> { EndPosition };

	public bool IsWall(Point p) => Walls.Contains(p);
	public Blizzard GetBlizzard(Point p) => Blizzards.FirstOrDefault(b => b.Position == p);
	public Blizzard[] GetBlizzards(Point p) => Blizzards.Where(b => b.Position == p).ToArray();

	public void MoveBlizards() => Blizzards.AsParallel().ForAll(b => b.Move());

	public void AddPossiblePositions()
	{
		PossiblePositions = PossiblePositions
			.SelectMany(p => new[]
				{
					p,
					new Point(p.X + 1, p.Y),
					new Point(p.X - 1, p.Y),
					new Point(p.X, p.Y + 1),
					new Point(p.X, p.Y - 1),
				}
				.Where(pp => pp.X >= 0 && pp.Y >= 0) // Prevent going outside entrance
				.Where(pp => pp.X <= MaxX && pp.Y <= MaxY) // Prevent going outside exit
				.Except(Walls)
			)
			.Distinct()
			.Except(Blizzards.Select(b => b.Position))
			.ToList();
	}

	public void Print()
	{
		var sb = new StringBuilder("<pre>");

		for (int y = MinY; y <= MaxY; y++)
		{
			for (int x = MinX; x <= MaxX; x++)
			{
				var p = new Point(x, y);
				if (IsWall(p)) sb.Append('#');
				else
				{
					var isPosition = PossiblePositions.Contains(p);
					var b = GetBlizzards(p);
					if (b.Length > 1)
						sb.Append(b.Length);
					else if (b.Length == 1)
						sb.Append(b[0].Direction switch
						{
							'<' => "&lt;",
							'>' => "&gt;",
							_ => b[0].Direction
						});
					else if (isPosition)
						sb.Append('E');
					else
						sb.Append('.');
				}
			}
			sb.AppendLine();
		}

		Util.RawHtml(sb.ToString()).Dump();
	}
}

public class Blizzard
{
	public Cavern Cavern { get; }
	public Point Position { get; private set; }
	public char Direction { get; private set; }

	public Blizzard(Cavern cavern, int x, int y, char direction)
	{
		Cavern = cavern;
		Position = new Point(x, y);
		Direction = direction;
	}

	public void Move()
	{
		var next = Direction switch
		{
			'>' => new Point(Position.X + 1, Position.Y),
			'<' => new Point(Position.X - 1, Position.Y),
			'^' => new Point(Position.X, Position.Y - 1),
			'v' => new Point(Position.X, Position.Y + 1),
			_ => throw new ArgumentOutOfRangeException()
		};

		if (next.X <= Cavern.MinX) next.X = Cavern.MaxX - 1;
		if (next.X >= Cavern.MaxX) next.X = Cavern.MinX + 1;
		if (next.Y <= Cavern.MinY) next.Y = Cavern.MaxY - 1;
		if (next.Y >= Cavern.MaxY) next.Y = Cavern.MinY + 1;

		Position = next;
	}
}

public enum Direction { Right = 0, Down = 1, Left = 2, Up = 3 }

public static string smallInput = @"#.#####
#.....#
#>....#
#.....#
#...v.#
#.....#
#####.#";

public static string sampleInput = @"#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#";

public static string puzzleInput = @"#.########################################################################################################################
#<^.>v<.vv.><<<v<vv<v^>^^v<>.^>.^v^>vv^^^><<^..>^^^>><.^>^v<v<<>.>.><.vv<^>v^<v^<v^v^><^vv^>^>^v><<v>^.^>v^v^^<..v>^..v^<#
#>^v>v^vvv^><v.v<v<.v<>^v^v.v<v..^<^<.^<^vv<><<<>.^v.v.<<^<v^^^<.<^<^v.vvv^vv.>v^>^v<>^vv^^<<v<.^<<>v>^>>vv<<v><^vvvv>^v>#
#>v^..vv><^v^^^><><^>>^<v.^.v^^^v^^<>v^v<<^><v<<v.v^^^>vv<><>v^<>^v>.vv^v<>>^<<v^.v<v^^<..>^v>^^v>^<^>^v>vv.>>v^.><>^.>v<#
#.>>.vv<.<<^v<^<<^^^><^<<^>v^v<><<v<><v>>^^<>vv>^>^vvvv^.>....>.v<^^^v^>.<^v.<vvv>v^>><>>><>.v.v^<v>^<^<v>>vv<vvvv>v<.<>>#
#<^<v>v<<v<<>v>v^<.^<v.^v^v>v<<v<><>.^.^.v>^^^<^<v.v.>>><<<^v<>>><v>^<^vv>><.><v<v<>^..vv>^v^v.v<v..<^v><>..<.<>^vvvv>v<<#
#<v>>><vvv>.>>^vv^<>>vv^<.<..<v>>>^>v>>^.^<.^..><<^v<.<<<<v.<^^^.<>^>><.^v^v.<><<^<><<v><^^.><v>^<^.^<>>>^<<<<<<>><^.vv^<#
#>v^>.v><><^><>>.<<^vvv^v^^..vv>><<^>^^<^v.><^v<^><<>>><>^^v<<.^.^<<v>^>.>.>><>^^><^^<^>^..v..<^^<>v^.>>>>^v<v>.<.^<^<<v<#
#<v.>v^<..vv^vv>>^<^>>v<<>><vv..<<<<v.v^>^v><^><v^><<>v>^>v.v>v.>v^<><^>>vv<>.>^^<<>.><>v>v.^v>^vv>v^.v>><v<^vv<<<v^><><>#
#<v^<.^^^^<<.^^^.<^.<v<<^vv.><^<<<v<<v<<>^^<<.<vvv><v>>.>>>^><.>^<>^<>^>^v^vvv>^^>v^>v>>.<v<<v><>v>v^^<v<v^<^>v.>><v^><>.#
#>.^..<<v>^><.^<<^^<>^v<.^<^>>v<^>^>>^<<^^><^><v><.>><<v>^v^.v^^^vv<<^<vv^>^>v^v<<v^v<<.v><vvv><<^<v.>>v.>v.v<.^v><>^<^<>#
#.v.<^v>>^<vv^>^.><<v^>.>v>^>>><<v>v<<^^>.^>v><>v<^v^.>>^>>^><<>^><>.^^<>^<><^v^><^^>v<^.<<.v>>^.v^^<^.<^.<^.>^<<>.^<>..>#
#.v>v>^^<^><><><v><.<>v>^.^.v<^>.v^v<v><vv.>>^>.v<v^><<>^v^.^v>v>^><>vv><<v><<v<>^^v^v<>vv>><<>vv<>^.<<.>v>v<<v>v<>><^vv<#
#>.^.v>^^.<<vv<^<^<>^v>v^v>>.<<^vv.<<>^^^<^^^<>v..<v>^>^<><^v><><>..vv<^.<>^^>vv^^<.<<><^v^><vv^>>vv.v>>^^<^<>vv><^<^^>><#
#<^.^>>.^.v>>v>.<v><>^vv<><<><<<<v<^v>v^vv>.<<<v<<>^><<>^^<>^>^>vv<<v<><^^>^<>v<v^^v>^v.vv<>v<>^<v<<^v^<>>^v<^v.>^^>.>>v.#
#<>>.^^><vv<vv^<>>^><>><^vv><^>^<<>vv><>><>^<^v^^^>>v<.vv>v^^.^vv<^^^<^>v<>>>vv>^v.<<>>>^>vv<<^<<<v<<^<<>^^v.v^v^<^<<><v<#
#<.<>v<^^<<>v>v^v>v>v..><>><^>><<v>.v<<>vv.^^<^^v^^^<.v.>.<.<>v^v^v^><<><^^>.>^^><><^^>.>.><^<^^<<v<.^^<vv>^><>>.>v^^^^v<#
#>^^<v>v>>v>^^><<^^^<v^<<<><^>>v^^><v>>>..><>>.^v<^<>>v<<^<^^^><.v.^^v<^.>v.<<.vv<<.^vv.<<^>>v>v^^v<v<<v<<<><><<<<^>^><.<#
#>><>.v>.v>.<^>><<.<>..v^<^v>v>>^.>^.v.^<vv>^<<vvv^vvv<>><^^.<<<<v^><.><>^^.<<<v>vv><<>>vv^>v>><<^>><^.v<<<^><<^vv<<..><<#
#<^><v^<^^>v><.><.^^<^>>>v<<^>^>^<<><.v^^v^v<^^v^^^<>^.<v><<^<>vv^>^^v^v<^vv.v^v<>>.v^>.v<<>><vvvvv<>^<<v>^^.v^>^^v^v<>.>#
#>^<<.^<.>.<>.^v^^<<v<.>><v.<>v<v^^>v>>v<<<^>vv<^vv^<><^v^.vv>v>^^>><^^v^>><>^>>v>v^>>v<vv.^>><<^<^.vv<><^^>>..v<v^vv><^<#
#<v><><^>>>^.>.v.^><..<v<<^>^v<>^>><^^v^><....v>>.>><>>v><vv<>^v^.><v^^^^^^v>v>.v<<v<^>><^v<^>^>.><^>vv>^>^v>.><>><v>vvv<#
#>^>v>v^^<vv^vv<vv>^^^.^><^>^vv.v^v>vvvvv>^^^>^>^<v><><><>v<<<<>^v^^<<.v^<^.^^^v<^^>^><<v<.<^v>>v.><<<>v^^v>>v^<^^^><>><<#
#>^<v^<vv.v<.^<vv.vv>^<.v>v^v>v<^.<<>.^vv.<<><v>vv<<>^.v.^<^^vv<v^^>^<v.v.<<.v^>v.^.^>v>v>^<>vvv>>^v<^^vvv.vv.<^v^><^^>><#
#>.^.vvv>vv><vvv^<^>^>.><<>v^v>vvv^v.v>>>.^<^^.^>>>>v.>v^>^>>><^>v^^<.>>>^<>v<>v.>>^v^vvv.v<^<<v<<^^vvv>>>^>^^.^>^>>.<<>.#
#<^v<^v<v.^.vv>v^>v^v.vv^<<>^<>^><>^<v^>^.>v.vv><^<.v^.^<vvvv^v>>^vv><<>>v>^v<<<.>vv<<^v^v>vvv>v..<><<v>^>>v^>^.^vv^<<<^<#
########################################################################################################################.#";