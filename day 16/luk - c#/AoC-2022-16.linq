<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

void Main()
{
	Run(sampleInput).Dump("sample (1651/1707)");
	Run(puzzleInput).Dump("puzzle (1728/-)");
}


public object Run(string input)
{
	var shortestPath = new ShortestPath(input);
	shortestPath.Print();

	return new
	{
		Part1 = FindMax(shortestPath),
		Part2 = FindMax2(shortestPath),
	};
}

public int FindMax(ShortestPath sp) => sp.FindAllPaths(30).Max(p => p.Value);
public int FindMax2(ShortestPath sp)
{
	var allPaths = sp.FindAllPaths(26).OrderByDescending(p => p.Value).ToList();
	
	foreach (var myPath in allPaths)
	{
		var findComplement = allPaths.FirstOrDefault(p => p.Path.Intersect(myPath.Path).Count() == 1);
		if (findComplement == null) continue;
		
		return findComplement.Value + myPath.Value;
	}

	return allPaths.First().Value;
}

public class ShortestPath
{
	private IDictionary<string, Valve> Valves { get; }
	private PathNode[,] Nodes { get; }

	public IEnumerable<Valve> ClosedValves => Valves.Values.Where(v => !v.IsOpen);
	public Valve this[string name] => Valves[name];
	public Valve this[int i] => Valves.Values.First(v => v.Index == i);
	public PathNode this[Valve from, Valve to] => Nodes[from.Index, to.Index];
	public PathNode this[int i, int j] => Nodes[i, j];

	public ShortestPath(string input) : this(Valve.Parse(input)) { }
	public ShortestPath(IDictionary<string, Valve> valves)
	{
		Valves = valves;
		Nodes = new PathNode[valves.Count, valves.Count];

		var queue = new List<Valve>();
		queue.Add(valves["AA"]);

		for (int i = 0; i < queue.Count; i++)
		{
			var current = queue[i];
			Nodes[current.Index, current.Index] = new PathNode(new[] { current });

			foreach (var targetName in current.Targets)
			{
				var target = valves[targetName];
				if (!queue.Contains(target)) queue.Add(target);

				Nodes[current.Index, target.Index] = new PathNode(new[] { current, target });
				Nodes[target.Index, current.Index] = new PathNode(new[] { target, current });
			}
		}

		for (int cycle = 1; cycle < valves.Count; cycle++)
		{
			var finished = true;
			for (int from = 0; from < Nodes.GetLength(0); from++)
				for (int to = 0; to < Nodes.GetLength(1); to++)
				{
					var node = Nodes[from, to];
					if (node == null)
					{
						finished = false;
						continue;
					}

					if (node.Length == cycle)
					{
						foreach (var targetName in node.To.Targets)
						{
							if (targetName == node.From.Name) continue;
							var target = valves[targetName];
							Nodes[node.From.Index, target.Index] ??= new PathNode(node, target, false);
							Nodes[target.Index, node.From.Index] ??= new PathNode(node, target, true);
						}
					}
				}

			if (finished) break;
		}
	}
	
	public IEnumerable<CalculatedPath> FindAllPaths(int maxTime) 
		=> FindAllPaths(new [] { this["AA"] }, 0, 0, 0, maxTime);
	
	private IEnumerable<CalculatedPath> FindAllPaths(IEnumerable<Valve> path, int time, int flowRate, int result, int maxTime)
	{
		if (time > maxTime) throw new Exception();

		var lastValve = path.Last();

		// travel to all closed valves (with flowrate) except those travelled to
		var possibleValves = ClosedValves.Except(path);
		if (!possibleValves.Any()) return new[] { new CalculatedPath(path, result + (flowRate * (maxTime - time))) };

		return possibleValves
			.SelectMany(v =>
			{
				var p = this[lastValve, v];

				if (time + p.Length >= maxTime)
				{
					//$"<= {v}: {p.Length} | {result + (flowRate * (30 - time))}".Dump();
					return new[] { new CalculatedPath(path, result + (flowRate * (maxTime - time))) };
				}

				return FindAllPaths(
					path.Concat(new[] { v }).ToList(),
					time + p.Length + 1,
					flowRate + v.FlowRate,
					result + (flowRate * (p.Length + 1)),
					maxTime
				);
			});
	}

	public void Print()
	{
		var sb = new StringBuilder("<pre>");

		sb.Append("   |");
		for (int x = 0; x < Nodes.GetLength(0); x++)
		{
			var p = Nodes[x, 0];
			if (p.From.IsOpen) continue;
			sb.Append($"{p.From.Name} |");
		}
		sb.AppendLine();

		for (int y = 0; y < Nodes.GetLength(1); y++)
		{
			var to = Nodes[0, y].To;
			if (to.IsOpen) continue;

			sb.Append($"{to.Name} |");

			var hasElements = false;
			for (int x = 0; x < Nodes.GetLength(0); x++)
			{
				var p = Nodes[x, y];
				if (p.From.IsOpen) continue;
				hasElements = true;
				sb.AppendFormat("{0:000}|", p.Length);
			}
			if (hasElements) sb.AppendLine();
		}
		sb.AppendLine();
		sb.Append("</pre>");
		Util.RawHtml(sb.ToString()).Dump();
	}
}

public class CalculatedPath
{
	public List<Valve> Path { get; set; }
	public int Value { get; set; }
	
	public CalculatedPath(IEnumerable<Valve> path, int value)
	{
		Path = new List<Valve>(path);
		Value = value;
	}
}

public class PathNode
{
	public List<Valve> Path { get; set; }
	public int Length => Path.Count - 1;
	public Valve From => Path[0];
	public Valve To => Path[^1];

	public PathNode(IEnumerable<Valve> path)
	{
		Path = new List<Valve>(path);
	}
	public PathNode(PathNode previous, Valve next, bool reverse)
		: this(previous.Path)
	{
		Path.Add(next);
		if (reverse) Path.Reverse();
	}
}

public class Valve
{
	private static int _seed = 0;

	public int Index { get; } = _seed++;
	public string Name { get; }
	public int FlowRate { get; }
	public HashSet<string> Targets { get; }
	public bool IsOpen { get; set; }

	public Valve(string input)
	{
		var match = Regex.Match(input, @"Valve (?<name>[A-Z][A-Z]) has flow rate=(?<flowRate>(-|)\d+); tunnel(s|) lead(s|) to valve(s|) (?<targets>.*)");

		Name = match.Groups["name"].Value;
		FlowRate = int.Parse(match.Groups["flowRate"].Value);
		Targets = new HashSet<string>(match.Groups["targets"].Value.Split(", "));
		IsOpen = FlowRate == 0;
	}

	public static Dictionary<string, Valve> Parse(string input)
	{
		_seed = 0;
		return input.Split("\r\n").Select(l => new Valve(l)).ToDictionary(v => v.Name);
	}

	public override string ToString() => $"{Name} ({FlowRate})";
}


public static string sampleInput = @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II";

public static string puzzleInput = @"Valve GJ has flow rate=14; tunnels lead to valves UV, AO, MM, UD, GM
Valve HE has flow rate=0; tunnels lead to valves QE, SV
Valve ET has flow rate=0; tunnels lead to valves LU, SB
Valve SG has flow rate=0; tunnels lead to valves FF, SB
Valve LC has flow rate=0; tunnels lead to valves QJ, GM
Valve EE has flow rate=13; tunnels lead to valves RE, BR
Valve AA has flow rate=0; tunnels lead to valves QC, ZR, NT, JG, FO
Valve TF has flow rate=0; tunnels lead to valves LU, MM
Valve GO has flow rate=0; tunnels lead to valves LB, AH
Valve QE has flow rate=24; tunnels lead to valves LG, HE
Valve MI has flow rate=0; tunnels lead to valves KU, FF
Valve BR has flow rate=0; tunnels lead to valves HY, EE
Valve UV has flow rate=0; tunnels lead to valves GP, GJ
Valve EH has flow rate=0; tunnels lead to valves UU, FF
Valve WK has flow rate=0; tunnels lead to valves HY, EL
Valve NT has flow rate=0; tunnels lead to valves FF, AA
Valve KI has flow rate=0; tunnels lead to valves OQ, AO
Valve AH has flow rate=22; tunnels lead to valves GO, RE
Valve EL has flow rate=0; tunnels lead to valves WK, SQ
Valve GP has flow rate=0; tunnels lead to valves SB, UV
Valve GM has flow rate=0; tunnels lead to valves LC, GJ
Valve LU has flow rate=9; tunnels lead to valves UU, DW, TF, ET, ML
Valve LB has flow rate=0; tunnels lead to valves GO, VI
Valve QC has flow rate=0; tunnels lead to valves ML, AA
Valve JJ has flow rate=0; tunnels lead to valves QJ, DV
Valve MM has flow rate=0; tunnels lead to valves TF, GJ
Valve VI has flow rate=18; tunnel leads to valve LB
Valve NV has flow rate=0; tunnels lead to valves SB, KU
Valve VT has flow rate=0; tunnels lead to valves HY, JG
Valve RE has flow rate=0; tunnels lead to valves AH, EE
Valve FO has flow rate=0; tunnels lead to valves SB, AA
Valve DV has flow rate=10; tunnels lead to valves JH, UD, JJ
Valve SQ has flow rate=12; tunnels lead to valves EL, QA
Valve OQ has flow rate=23; tunnels lead to valves KI, IV, JS
Valve FF has flow rate=3; tunnels lead to valves EU, NT, SG, MI, EH
Valve IV has flow rate=0; tunnels lead to valves LG, OQ
Valve HY has flow rate=8; tunnels lead to valves VT, BR, WK
Valve ML has flow rate=0; tunnels lead to valves LU, QC
Valve JS has flow rate=0; tunnels lead to valves EM, OQ
Valve KU has flow rate=5; tunnels lead to valves MI, VL, NV, HU, DW
Valve QA has flow rate=0; tunnels lead to valves OS, SQ
Valve EU has flow rate=0; tunnels lead to valves FF, OS
Valve SV has flow rate=0; tunnels lead to valves QJ, HE
Valve JG has flow rate=0; tunnels lead to valves AA, VT
Valve DW has flow rate=0; tunnels lead to valves LU, KU
Valve UD has flow rate=0; tunnels lead to valves DV, GJ
Valve QJ has flow rate=17; tunnels lead to valves JJ, SV, LC, EM, YA
Valve HU has flow rate=0; tunnels lead to valves JH, KU
Valve ZR has flow rate=0; tunnels lead to valves AA, VL
Valve YA has flow rate=0; tunnels lead to valves QJ, OS
Valve JH has flow rate=0; tunnels lead to valves HU, DV
Valve OS has flow rate=15; tunnels lead to valves EU, YA, QA
Valve LG has flow rate=0; tunnels lead to valves QE, IV
Valve SB has flow rate=4; tunnels lead to valves FO, SG, NV, GP, ET
Valve UU has flow rate=0; tunnels lead to valves EH, LU
Valve VL has flow rate=0; tunnels lead to valves ZR, KU
Valve AO has flow rate=0; tunnels lead to valves GJ, KI
Valve EM has flow rate=0; tunnels lead to valves QJ, JS";