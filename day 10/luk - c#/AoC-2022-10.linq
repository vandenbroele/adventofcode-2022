<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	//Run(sample, true).Dump("sample 1");
	Run(sample2, true).Dump("sample 2");
	Run(input, false).Dump("result");
}

public static object Run(string input, bool debug)
{
	var x = 1;
	var opQueue = new Queue<IOp>(input.Split("\r\n").Select(IOp.Parse));

	IOp currentOp = new NoOp(true);
	long signalStrength = 0;

	var crt = new Crt();

	for (int cycle = 1; cycle <= 240; cycle++)
	{
		if (opQueue.Count > 0 && currentOp.Done)
			currentOp = opQueue.Dequeue();

		if (cycle == 20 || (cycle - 20) % 40 == 0)
		{
			signalStrength += x * cycle;
			if (debug) $"Signal strength ({cycle}, {x}) = {x * cycle}".Dump();
		}
		crt.Render(cycle, x);
		if (!currentOp.Done) currentOp.Run(ref x);
	}

	crt.Print();

	return new
	{
		Part1 = signalStrength,
		Part2 = 0
	};
}


public class Crt
{
	public char[,] Grid { get; }

	public Crt()
	{
		Grid = new char[6, 40];

		for (int i = 0; i < Grid.GetLength(0); i++)
			for (int j = 0; j < Grid.GetLength(1); j++)
			{
				Grid[i, j] = ' ';
			}
	}

	public void Render(int cycle, int x)
	{
		int i = (cycle - 1) / 40;
		int j = (cycle - 1) % 40;

		x -= 1;
		if (j >= x && j < (x + 3))
			Grid[i, j] = '#';
	}

	public void Print()
	{
		var sb = new StringBuilder("<pre>");
		for (int i = 0; i < Grid.GetLength(0); i++)
		{
			for (int j = 0; j < Grid.GetLength(1); j++)
				sb.Append(Grid[i, j]);

			sb.Append("<br>");
		}
		sb.Append("</pre>");
		Util.RawHtml(sb.ToString()).Dump();
	}
}


public interface IOp
{
	public bool Done { get; }
	public void Run(ref int x);

	public static IOp Parse(string value)
	{
		if (value == "noop") return new NoOp();

		if (value.StartsWith("add")) return AddOp.Parse(value);

		throw new InvalidOperationException();
	}
}

public class NoOp : IOp
{
	public bool Done { get; protected set; }
	public void Run(ref int x) => Done = true;

	public NoOp() { }
	public NoOp(bool done) { Done = done; }

	public override string ToString() => "noop";
}

public class AddOp : IOp
{
	public bool Done { get; protected set; }
	private int _runCycle = 0;

	public string Register { get; private set; }
	public int Value { get; private set; }

	public void Run(ref int x)
	{
		if (Done) return;
		if (_runCycle == 0)
		{
			_runCycle++;
			return;
		}

		x += Value;
		Done = true;
	}
	public override string ToString() => $"add{Register} {Value}";

	public static AddOp Parse(string value) => new AddOp
	{
		Register = value.Substring(3, 1),
		Value = int.Parse(value.Substring(4))
	};
}


public static string sample = @"noop
addx 3
addx -5";
public static string sample2 = @"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop";

public static string input = @"addx 2
addx 4
noop
noop
addx 17
noop
addx -11
addx -1
addx 4
noop
noop
addx 6
noop
noop
addx -14
addx 19
noop
addx 4
noop
noop
addx 1
addx 4
addx -20
addx 21
addx -38
noop
addx 7
noop
addx 3
noop
addx 22
noop
addx -17
addx 2
addx 3
noop
addx 2
addx 3
noop
addx 2
addx -8
addx 9
addx 2
noop
noop
addx 7
addx 2
addx -27
addx -10
noop
addx 37
addx -34
addx 30
addx -29
addx 9
noop
addx 2
noop
noop
noop
addx 5
addx -4
addx 9
addx -2
addx 7
noop
noop
addx 1
addx 4
addx -1
noop
addx -19
addx -17
noop
addx 1
addx 4
addx 3
addx 11
addx 17
addx -23
addx 2
noop
addx 3
addx 2
addx 3
addx 4
addx -22
noop
addx 27
addx -32
addx 14
addx 21
addx 2
noop
addx -37
noop
addx 31
addx -26
addx 5
addx 2
addx 3
addx -2
addx 2
addx 5
addx 2
addx 3
noop
addx 2
addx 9
addx -8
addx 2
addx 11
addx -4
addx 2
addx -15
addx -22
addx 1
addx 5
noop
noop
noop
noop
noop
addx 4
addx 19
addx -15
addx 1
noop
noop
addx 6
noop
noop
addx 5
addx -1
addx 5
addx -14
addx -13
addx 30
noop
addx 3
noop
noop";