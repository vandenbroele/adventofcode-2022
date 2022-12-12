<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	Run(sample);
	Run(input);
}

public void Run(string input)
{
	var grid = new Grid(input);
	grid.ShortestPathUp().Dump("Part 1");
	grid.ShortestPathDown().Dump("Part 2");
}

public class Grid
{
	private Point _start;
	private Point _end;
	private char[,] _grid;
	private int[,] _result;

	public Grid(string input)
	{
		var lines = input.Split("\r\n");
		for (int i = 0; i < lines.Length; i++)
		{
			_grid ??= new char[lines.Length, lines[i].Length];
			_result ??= new int[lines.Length, lines[i].Length];

			for (int j = 0; j < lines[i].Length; j++)
			{
				_grid[i, j] = lines[i][j];
				_result[i, j] = -1;

				if (IsStart(_grid[i, j])) _start = new Point(j, i);
				if (IsEnd(_grid[i, j])) _end = new Point(j, i);
			}
		}
	}

	public int ShortestPathUp()
	{
		for (int i = 0; i < _grid.GetLength(1); i++)
			for (int j = 0; j < _grid.GetLength(0); j++)
				_result[j, i] = -1;
				
		_result[_start.Y, _start.X] = 0;
		
		return RunDijkstra();
	}

	public int ShortestPathDown()
	{
		for (int i = 0; i < _grid.GetLength(1); i++)
			for (int j = 0; j < _grid.GetLength(0); j++)
				_result[j, i] = _grid[j, i] == 'a' ? 0 : -1;

		_result[_start.Y, _start.X] = 0;
		
		return RunDijkstra();
	}
	
	private int RunDijkstra()
	{
		var cycle = 0;
		while (_result[_end.Y, _end.X] < 0)
		{
			for (int i = 0; i < _grid.GetLength(1); i++)
				for (int j = 0; j < _grid.GetLength(0); j++)
				{
					if (_result[j, i] != cycle) continue;
					
					foreach (var neighbour in GetNeighbours(i, j))
						_result[neighbour.Y, neighbour.X] = _result[j, i] + 1;
				}
				
			cycle++;
		}

		return _result[_end.Y, _end.X];
	}

	private IEnumerable<Point> GetNeighbours(int x, int y) => GetNeighbours(new Point(x, y));
	private IEnumerable<Point> GetNeighbours(Point from)
	{
		Point? to;
		if ((to = CanClimbTo(from, 1, 0)).HasValue) yield return to.Value;
		if ((to = CanClimbTo(from, 0, 1)).HasValue) yield return to.Value;
		if ((to = CanClimbTo(from, -1, 0)).HasValue) yield return to.Value;
		if ((to = CanClimbTo(from, 0, -1)).HasValue) yield return to.Value;
	}

	private Point? CanClimbTo(Point from, int dx, int dy)
	{
		var to = new Point(from.X + dx, from.Y + dy);

		if (to.X < 0 || to.X > _grid.GetLength(1) - 1 || to.Y < 0 || to.Y > _grid.GetLength(0) - 1)
			return null;

		return Level(to) - Level(from) <= 1 
			&& _result[to.Y, to.X] < 0 ? to : null;
	}

	private char GetValue(Point p) => _grid[p.Y, p.X];
	private bool IsStart(Point p) => IsStart(GetValue(p));
	private bool IsEnd(Point p) => IsEnd(GetValue(p));
	private int Level(Point p) => Level(GetValue(p));

	private bool IsStart(char c) => c == 'S';
	private bool IsEnd(char c) => c == 'E';
	private int Level(char c) => IsStart(c) ? 'a' : IsEnd(c) ? 'z' : c;
}


public static string sample = @"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi";

public static string input = @"abacccaaaacccccccccccaaaaaacccccaaaaaaccccaaacccccccccccccccccccccccccccccccccccccccccccaaaaa
abaaccaaaacccccccccccaaaaaaccccccaaaaaaaaaaaaaccccccccccccccccccccccccccccccccccccccccccaaaaa
abaaccaaaacccccccccccaaaaacccccaaaaaaaaaaaaaaaccccccccccccccccccccccccccccccccccccccccccaaaaa
abccccccccccccccccccccaaaaacccaaaaaaaaaaaaaaaacccccccccccccccccccccccccccaaaccccccccccccaaaaa
abccccccccccccccccccccaacaacccaaaaaaaaccaaaaaccccccccccccccccccccccccccccaaaccccccccccccaccaa
abcccccccccccccaacccaaaccccccaaaaaaaaaccaaaaaccccccccccccccccccccccccccccccacccccccccccccccca
abcccccccccccaaaaaaccaaaccacccccaaaaaaacccccccccccccccccccccccccciiiicccccccddddddccccccccccc
abcccccccccccaaaaaaccaaaaaaaccccaaaaaacccccaacccccccaaaccccccccciiiiiiiicccdddddddddacaaccccc
abccccccccccccaaaaaaaaaaaaacccccaaaaaaacaaaacccccccaaaacccccccchhiiiiiiiiicddddddddddaaaccccc
abcccccccccccaaaaaaaaaaaaaacccccccaaacccaaaaaacccccaaaaccccccchhhipppppiiiijjjjjjjddddaaccccc
abcccccccccccaaaaaaaaaaaaaaccccccccccccccaaaaaccccccaaaccccccchhhpppppppiijjjjjjjjjddeeaccccc
abcccccccccccccccccaaaaaaaacccccccccccccaaaaaccccccccccccccccchhppppppppppjjqqqjjjjjeeeaacccc
abccccccccccccccccccaaaaaaaacccccccccccccccaacccccccccccccccchhhpppuuuupppqqqqqqqjjjeeeaacccc
abcccccccccccccccccccaacccacccccccccccccccccccccccccccccccccchhhopuuuuuuppqqqqqqqjjjeeecccccc
abacccccccccccccaaacaaaccccccccccccccccccccccccccccaaccccccchhhhoouuuuuuuqvvvvvqqqjkeeecccccc
abaccccccccccccaaaaaacccccaaccccccccccccccccccccccaaaccccccchhhooouuuxxxuvvvvvvqqqkkeeecccccc
abaccccccccccccaaaaaacccaaaaaaccccccccccccccccccaaaaaaaaccchhhhooouuxxxxuvyyyvvqqqkkeeecccccc
abcccccccccccccaaaaacccaaaaaaaccccccccccccccccccaaaaaaaaccjjhooooouuxxxxyyyyyvvqqqkkeeecccccc
abccccccccccccccaaaaaacaaaaaaaccccccccaaaccccccccaaaaaaccjjjooootuuuxxxxyyyyyvvqqkkkeeecccccc
abccccccccccccccaaaaaaaaaaaaacccccccccaaaacccccccaaaaaacjjjooootttuxxxxxyyyyvvrrrkkkeeecccccc
SbccccccccccccccccccaaaaaaaaacccccccccaaaacccccccaaaaaacjjjoootttxxxEzzzzyyvvvrrrkkkfffcccccc
abcccccccccccaaacccccaaaaaaacaaaccccccaaaccccccccaaccaacjjjoootttxxxxxyyyyyyvvvrrkkkfffcccccc
abcccccccccaaaaaacccaaaaaacccaaacacccaacccccccccccccccccjjjoootttxxxxyxyyyyyywvvrrkkkfffccccc
abcccccccccaaaaaacccaaaaaaaaaaaaaaaccaaacaaacccccaacccccjjjnnnttttxxxxyyyyyyywwwrrkkkfffccccc
abcaacacccccaaaaacccaaacaaaaaaaaaaaccaaaaaaacccccaacaaacjjjnnnntttttxxyywwwwwwwwrrrlkfffccccc
abcaaaaccccaaaaacccccccccaacaaaaaaccccaaaaaacccccaaaaacccjjjnnnnnttttwwywwwwwwwrrrrllfffccccc
abaaaaaccccaaaaaccccccaaaaaccaaaaacaaaaaaaaccccaaaaaaccccjjjjinnnntttwwwwwsssrrrrrllllffccccc
abaaaaaaccccccccccccccaaaaacaaaaaacaaaaaaaaacccaaaaaaacccciiiiinnnntswwwwssssrrrrrlllfffccccc
abacaaaaccccccccccccccaaaaaacaaccccaaaaaaaaaaccccaaaaaaccccciiiinnnssswwsssssllllllllfffccccc
abccaaccccccccccccccccaaaaaaccccccccccaaacaaaccccaaccaacccccciiiinnsssssssmmllllllllfffaacccc
abccccccccccccccccccccaaaaaaccccccccccaaaccccccccaaccccccccccciiinnmsssssmmmmlllllgggffaacccc
abcccccccccccccccaccccccaaacccccccccccaaccccccccccccccccccccccciiimmmsssmmmmmgggggggggaaacccc
abcccccccccaaaaaaaaccccccccccccccccccccccccccccaaaaaccccccccccciiimmmmmmmmmgggggggggaaacccccc
abccccccccccaaaaaaccccccccccccccccccaacccccccccaaaaacccccccccccciiimmmmmmmhhggggcaaaaaaaccccc
abccccccccccaaaaaacccccccccccccccccaacccccccccaaaaaacccccccccccciihhmmmmhhhhgccccccccaacccccc
abccccaacaaaaaaaaaaccccccccccccccccaaaccccccccaaaaaaccccccccccccchhhhhhhhhhhaaccccccccccccccc
abccccaaaaaaaaaaaaaaccccccccccaaccaaaaccccccccaaaaaacccaaacccccccchhhhhhhhaaaaccccccccccccccc
abcccaaaaaaaaaaaaaaaccccccccaaaaaacaaaacacaccccaaaccccaaaacccccccccchhhhccccaaccccccccccaaaca
abcccaaaaaacacaaacccccccccccaaaaaaaaaaaaaaacccccccccccaaaacccccccccccaaaccccccccccccccccaaaaa
abcccccaaaacccaaaccccccccccaaaaaaaaaaaaaaaaccccccccccccaaacccccccccccaaacccccccccccccccccaaaa
abcccccaacccccaacccccccccccaaaaaaaaaaaaaccccccccccccccccccccccccccccccccccccccccccccccccaaaaa";
