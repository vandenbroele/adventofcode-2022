<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	var sampleGrid = new Grid(sample);
	//sampleGrid.PrintSensor(8, 7);
	//sampleGrid.Print(any: true);

	var sampleUsedPoints = sampleGrid.CalculateUsedInLine(10)
						 - sampleGrid.All.Count(p => p.Y == 10);
	var sampleSignalLocation = sampleGrid.FindSignal(0, 20);

	new
	{
		Used = sampleUsedPoints,
		Frequency = SignalFrequency(sampleSignalLocation)
	}.Dump("sample");

	var puzzleGrid = new Grid(puzzleInput);
	var puzzleUsedPoints = puzzleGrid.CalculateUsedInLine(2000000) 
						 - puzzleGrid.All.Count(p => p.Y == 2000000);
	var puzzleSignalLocation = puzzleGrid.FindSignal(0, 4000000);

	new
	{
		Used = puzzleUsedPoints,
		Frequency = SignalFrequency(puzzleSignalLocation)
	}.Dump("puzzle");
}

private long SignalFrequency(Point p) => p.X * 4000000L + p.Y;
public static int Distance(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

public class Sensor
{
	public Point Location { get; }
	public Point Beacon { get; }
	public int Distance { get; }

	public Sensor(string input)
	{
		var match = Regex.Match(input, @"Sensor at x=(?<sx>(-|)\d+), y=(?<sy>(-|)\d+): closest beacon is at x=(?<bx>(-|)\d+), y=(?<by>(-|)\d+)");

		Location = new Point(int.Parse(match.Groups["sx"].Value), int.Parse(match.Groups["sy"].Value));
		Beacon = new Point(int.Parse(match.Groups["bx"].Value), int.Parse(match.Groups["by"].Value));
		Distance = Distance(Location, Beacon);
	}

	public Sensor(Point location, Point beacon)
	{
		Location = location;
		Beacon = beacon;
		Distance = Distance(Location, Beacon);
	}

	public bool IsInRange(int x, int y) => IsInRange(new Point(x, y));
	public bool IsInRange(Point p) => Distance >= Distance(Location, p);

	public Range RangeInLine(int y)
	{
		var yDistance = Math.Abs(Location.Y - y);
		if (yDistance > Distance) return null;
		
		var xDistance = Distance - yDistance;

		return new Range(Location.X - xDistance, Location.X + xDistance);
	}
}
public class Range
{
	public int X1 { get; set; }
	public int X2 { get; set; }
	
	public int Length => Math.Abs(X1 - X2) + 1;
	
	public Range(Range r) : this(r.X1, r.X2) { }
	public Range(int x1, int x2)
	{
		X1 = x1;
		X2 = x2;
	}
}

public class Grid
{
	private Dictionary<Point, Sensor> _sensors;

	public IEnumerable<Point> Sensors => _sensors.Keys;
	public IEnumerable<Point> Beacons => _sensors.Values.Select(v => v.Beacon);
	public IEnumerable<Point> All => _sensors.SelectMany(k => new[] { k.Key, k.Value.Beacon });

	public int MinX => All.Min(p => p.X) - _sensors.Max(s => s.Value.Distance);
	public int MaxX => All.Max(p => p.X) + _sensors.Max(s => s.Value.Distance);
	public int MinY => All.Min(p => p.Y) - _sensors.Max(s => s.Value.Distance);
	public int MaxY => All.Max(p => p.Y) + _sensors.Max(s => s.Value.Distance);

	public Grid(string input)
	{
		_sensors = input.Split("\r\n")
			.Select(l => new Sensor(l))
			.ToDictionary(k => k.Location, k => k);
	}

	private List<Range> CalculateRangesInLine(int y, int min, int max)
	{
		var ranges = _sensors.Values
			.Select(s => s.RangeInLine(y))
			.Where(s => s != null)
			.OrderBy(s => s.X1)
			.ToList();

		var reduced = new List<Range>();

		var current = new Range(ranges[0]);
		reduced.Add(current);
		foreach (var range in ranges)
		{
			if (range.X1 <= current.X2)
				current.X2 = Math.Max(current.X2, range.X2);
			else
			{
				current = new Range(range);
				reduced.Add(current);
			}
		}
		return reduced;
	}

	public int CalculateUsedInLine(int y) => CalculateUsedInLine(y, MinX, MaxX);
	public int CalculateUsedInLine(int y, int min, int max)
	{
		return CalculateRangesInLine(y, min, max).Dump()
			.Select(r => new Range(Math.Max(r.X1, min), Math.Min(r.X2, max)))
			.Sum(r => r.Length);
	}

	public Point FindSignal(int min, int max)
	{		
		for (int y = min; y < max; y++)
		{
			var ranges = CalculateRangesInLine(y, min, max);
			if(ranges.Count > 1)
				return new Point(ranges[0].X2.Dump() + 1, y.Dump());
		}

		throw new Exception();
	}

	public bool IsInRangeOfAny(int x, int y) => IsInRangeOfAny(new Point(x, y));
	public bool IsInRangeOfAny(Point p) => _sensors.Any(s => s.Value.IsInRange(p));

	public bool IsSensor(int x, int y) => IsSensor(new Point(x, y));
	public bool IsSensor(Point p) => Sensors.Contains(p);
	public bool IsBeacon(int x, int y) => IsBeacon(new Point(x, y));
	public bool IsBeacon(Point p) => Beacons.Contains(p);

	public void PrintSensor(int px, int py)
	{
		var s = new Point(px, py);
		var b = _sensors[s];

		var points = new List<Point>();
		for (int y = MinY; y < MaxY; y++)
		{
			for (int x = MinX; x < MaxX; x++)
			{
				if (b.IsInRange(x, y)) points.Add(new Point(x, y));
			}
		}
		Print(points);
	}
	public void Print(IList<Point> points = null, bool any = false, string msg = null)
	{
		var sb = new StringBuilder("<pre style='1px solid #ccc'>");
		for (int y = MinY; y < MaxY; y++)
		{
			for (int x = MinX; x < MaxX; x++)
			{
				var p = new Point(x, y);
				if (IsSensor(p))
					sb.Append("S");
				else if (IsBeacon(p))
					sb.Append("B");
				else if (points?.Contains(p) ?? (any && IsInRangeOfAny(p)))
					sb.Append("#");
				else
					sb.Append(".");
			}

			sb.Append("<br>");
		}
		sb.Append("</pre>");
		Util.RawHtml(sb.ToString()).Dump(msg);
	}
}

public static string sample = @"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3";

public static string puzzleInput = @"Sensor at x=251234, y=759482: closest beacon is at x=-282270, y=572396
Sensor at x=2866161, y=3374117: closest beacon is at x=2729330, y=3697325
Sensor at x=3999996, y=3520742: closest beacon is at x=3980421, y=3524442
Sensor at x=3988282, y=3516584: closest beacon is at x=3980421, y=3524442
Sensor at x=3005586, y=3018139: closest beacon is at x=2727127, y=2959718
Sensor at x=3413653, y=3519082: closest beacon is at x=3980421, y=3524442
Sensor at x=2900403, y=187208: closest beacon is at x=2732772, y=2000000
Sensor at x=1112429, y=3561166: closest beacon is at x=2729330, y=3697325
Sensor at x=3789925, y=3283328: closest beacon is at x=3980421, y=3524442
Sensor at x=3991533, y=3529053: closest beacon is at x=3980421, y=3524442
Sensor at x=3368119, y=2189371: closest beacon is at x=2732772, y=2000000
Sensor at x=2351157, y=2587083: closest beacon is at x=2727127, y=2959718
Sensor at x=3326196, y=2929990: closest beacon is at x=3707954, y=2867627
Sensor at x=3839244, y=1342691: closest beacon is at x=3707954, y=2867627
Sensor at x=2880363, y=3875503: closest beacon is at x=2729330, y=3697325
Sensor at x=1142859, y=1691416: closest beacon is at x=2732772, y=2000000
Sensor at x=3052449, y=2711719: closest beacon is at x=2727127, y=2959718
Sensor at x=629398, y=214610: closest beacon is at x=-282270, y=572396
Sensor at x=3614706, y=3924106: closest beacon is at x=3980421, y=3524442
Sensor at x=3999246, y=2876762: closest beacon is at x=3707954, y=2867627
Sensor at x=3848935, y=3020496: closest beacon is at x=3707954, y=2867627
Sensor at x=123637, y=2726215: closest beacon is at x=-886690, y=3416197
Sensor at x=4000000, y=3544014: closest beacon is at x=3980421, y=3524442
Sensor at x=2524955, y=3861248: closest beacon is at x=2729330, y=3697325
Sensor at x=2605475, y=3152151: closest beacon is at x=2727127, y=2959718";