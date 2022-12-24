<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

void Main()
{
	Run(sampleInput);
	Run(puzzleInput);
}

public void Run(string input)
{
	var blueprints = BluePrint.Parse(input);


	new
	{
		QualityLevel = blueprints
			.Take(3)
			.AsParallel()
			.Select(bp => CalcQL(bp))
			.Aggregate((a, b) => a * b)
	}.Dump();
}

public int CalcQL(BluePrint blueprint)
{
	const int maxLevel = 33;

	var packs = new Stack<Pack>();
	packs.Push(new Pack());

	var maxOreCost = new[]
	{
		blueprint.OreRobot.OreCost,
		blueprint.ClayRobot.OreCost,
		blueprint.ObsidianRobot.OreCost,
		blueprint.GeodeRobot.OreCost
	}.Max();


	var nrOfGeodesAtLevel = Enumerable.Repeat(0, maxLevel).ToArray();
	while (packs.Count > 0)
	{
		var pack = packs.Pop();

		if (pack.Level >= maxLevel) continue;

		if (pack.Geode < nrOfGeodesAtLevel[pack.Level])
			continue;
		if (nrOfGeodesAtLevel[pack.Level] < pack.Geode)
			nrOfGeodesAtLevel[pack.Level] = pack.Geode;

		if (pack.CanCreateGeodeRobot(blueprint))
			packs.Push(pack.CopyCreateGeodeRobot(blueprint));

		packs.Push(pack.CopyCollect());

		if (pack.OreRobots <= maxOreCost && pack.CanCreateOreRobot(blueprint))
			packs.Push(pack.CopyCreateOreRobot(blueprint));

		if (pack.ClayRobots <= blueprint.ObsidianRobot.ClayCost && pack.CanCreateClayRobot(blueprint))
			packs.Push(pack.CopyCreateClayRobot(blueprint));

		if (pack.ObsidianRobots <= blueprint.GeodeRobot.ObsidianCost && pack.CanCreateObsidianRobot(blueprint))
			packs.Push(pack.CopyCreateObsidianRobot(blueprint));
	}

	return nrOfGeodesAtLevel
		.Max()
		.Dump($"blueprint {blueprint.Name}");// * blueprint.Name;
}

public class Pack
{
	public int Level { get; set; }
	public int OreRobots { get; set; } = 1;
	public int ClayRobots { get; set; }
	public int ObsidianRobots { get; set; }
	public int GeodeRobots { get; set; }

	public int Ore { get; set; }
	public int Clay { get; set; }
	public int Obsidian { get; set; }
	public int Geode { get; set; }

	private void Collect()
	{
		Ore += OreRobots;
		Clay += ClayRobots;
		Obsidian += ObsidianRobots;
		Geode += GeodeRobots;
		Level++;
	}
	public Pack CopyCollect()
	{
		var copy = (Pack)MemberwiseClone();
		copy.Collect();
		return copy;
	}

	public bool CanCreateOreRobot(BluePrint blueprint) => Ore >= blueprint.OreRobot.OreCost;
	public Pack CopyCreateOreRobot(BluePrint blueprint)
	{
		var copy = (Pack)MemberwiseClone();
		copy.Ore -= blueprint.OreRobot.OreCost;
		copy.Collect();
		copy.OreRobots++;
		return copy;
	}

	public bool CanCreateClayRobot(BluePrint blueprint) => Ore >= blueprint.ClayRobot.OreCost;
	public Pack CopyCreateClayRobot(BluePrint blueprint)
	{
		var copy = (Pack)MemberwiseClone();
		copy.Ore -= blueprint.ClayRobot.OreCost;
		copy.Collect();
		copy.ClayRobots++;
		return copy;
	}

	public bool CanCreateObsidianRobot(BluePrint blueprint) => Ore >= blueprint.ObsidianRobot.OreCost && Clay >= blueprint.ObsidianRobot.ClayCost;
	public Pack CopyCreateObsidianRobot(BluePrint blueprint)
	{
		var copy = (Pack)MemberwiseClone();
		copy.Ore -= blueprint.ObsidianRobot.OreCost;
		copy.Clay -= blueprint.ObsidianRobot.ClayCost;
		copy.Collect();
		copy.ObsidianRobots++;
		return copy;
	}

	public bool CanCreateGeodeRobot(BluePrint blueprint) => Ore >= blueprint.GeodeRobot.OreCost && Obsidian >= blueprint.GeodeRobot.ObsidianCost;
	public Pack CopyCreateGeodeRobot(BluePrint blueprint)
	{
		var copy = (Pack)MemberwiseClone();
		copy.Ore -= blueprint.GeodeRobot.OreCost;
		copy.Obsidian -= blueprint.GeodeRobot.ObsidianCost;
		copy.Collect();
		copy.GeodeRobots++;
		return copy;
	}
}

public class BluePrint
{
	public int Name { get; set; }
	public OreRobot OreRobot { get; set; }
	public ClayRobot ClayRobot { get; set; }
	public ObsidianRobot ObsidianRobot { get; set; }
	public GeodeRobot GeodeRobot { get; set; }

	public BluePrint(string line)
	{
		var match = Regex.Match(line,
		@"Blueprint (?<Name>\d+):" +
		@" Each ore robot costs (?<OreRobotOreCost>\d+) ore." +
		@" Each clay robot costs (?<ClayRobotOreCost>\d+) ore." +
		@" Each obsidian robot costs (?<ObsidianRobotOreCost>\d+) ore and (?<ObsidianRobotClayCost>\d+) clay." +
		@" Each geode robot costs (?<GeodeRobotOreCost>\d+) ore and (?<GeodeRobotObsidianCost>\d+) obsidian.");

		Name = int.Parse(match.Groups["Name"].Value);

		OreRobot = new OreRobot(int.Parse(match.Groups["OreRobotOreCost"].Value));
		ClayRobot = new ClayRobot(int.Parse(match.Groups["ClayRobotOreCost"].Value));

		ObsidianRobot = new ObsidianRobot(
			int.Parse(match.Groups["ObsidianRobotOreCost"].Value),
			int.Parse(match.Groups["ObsidianRobotClayCost"].Value)
		);
		GeodeRobot = new GeodeRobot(
			int.Parse(match.Groups["GeodeRobotOreCost"].Value),
			int.Parse(match.Groups["GeodeRobotObsidianCost"].Value)
		);
	}

	public static BluePrint[] Parse(string input) => input.Split("\r\n").Select(l => new BluePrint(l)).ToArray();
}
public record OreRobot(int OreCost);
public record ClayRobot(int OreCost);
public record ObsidianRobot(int OreCost, int ClayCost);
public record GeodeRobot(int OreCost, int ObsidianCost);

public static string sampleInput = @"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.";

public static string puzzleInput = @"Blueprint 1: Each ore robot costs 2 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 15 clay. Each geode robot costs 2 ore and 20 obsidian.
Blueprint 2: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 20 clay. Each geode robot costs 2 ore and 8 obsidian.
Blueprint 3: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 4 ore and 8 obsidian.
Blueprint 4: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 15 clay. Each geode robot costs 2 ore and 13 obsidian.
Blueprint 5: Each ore robot costs 2 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 20 clay. Each geode robot costs 4 ore and 18 obsidian.
Blueprint 6: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 17 clay. Each geode robot costs 2 ore and 13 obsidian.
Blueprint 7: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 12 clay. Each geode robot costs 4 ore and 19 obsidian.
Blueprint 8: Each ore robot costs 3 ore. Each clay robot costs 4 ore. Each obsidian robot costs 2 ore and 15 clay. Each geode robot costs 2 ore and 13 obsidian.
Blueprint 9: Each ore robot costs 3 ore. Each clay robot costs 3 ore. Each obsidian robot costs 2 ore and 9 clay. Each geode robot costs 2 ore and 9 obsidian.
Blueprint 10: Each ore robot costs 3 ore. Each clay robot costs 3 ore. Each obsidian robot costs 2 ore and 12 clay. Each geode robot costs 2 ore and 10 obsidian.
Blueprint 11: Each ore robot costs 3 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 10 clay. Each geode robot costs 2 ore and 13 obsidian.
Blueprint 12: Each ore robot costs 3 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 6 clay. Each geode robot costs 3 ore and 16 obsidian.
Blueprint 13: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 4 ore and 5 clay. Each geode robot costs 3 ore and 19 obsidian.
Blueprint 14: Each ore robot costs 3 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 20 clay. Each geode robot costs 4 ore and 16 obsidian.
Blueprint 15: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 8 clay. Each geode robot costs 2 ore and 18 obsidian.
Blueprint 16: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 2 ore and 10 clay. Each geode robot costs 4 ore and 10 obsidian.
Blueprint 17: Each ore robot costs 2 ore. Each clay robot costs 2 ore. Each obsidian robot costs 2 ore and 17 clay. Each geode robot costs 2 ore and 10 obsidian.
Blueprint 18: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 11 clay. Each geode robot costs 3 ore and 14 obsidian.
Blueprint 19: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 2 ore and 13 clay. Each geode robot costs 2 ore and 10 obsidian.
Blueprint 20: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 5 clay. Each geode robot costs 2 ore and 10 obsidian.
Blueprint 21: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 4 ore and 15 obsidian.
Blueprint 22: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 4 ore and 20 clay. Each geode robot costs 2 ore and 15 obsidian.
Blueprint 23: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 2 ore and 5 clay. Each geode robot costs 2 ore and 10 obsidian.
Blueprint 24: Each ore robot costs 3 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 5 clay. Each geode robot costs 4 ore and 8 obsidian.
Blueprint 25: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 17 clay. Each geode robot costs 3 ore and 10 obsidian.
Blueprint 26: Each ore robot costs 3 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 6 clay. Each geode robot costs 2 ore and 20 obsidian.
Blueprint 27: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 5 clay. Each geode robot costs 3 ore and 15 obsidian.
Blueprint 28: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 17 clay. Each geode robot costs 4 ore and 20 obsidian.
Blueprint 29: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 2 ore and 14 clay. Each geode robot costs 3 ore and 17 obsidian.
Blueprint 30: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 9 clay. Each geode robot costs 3 ore and 9 obsidian.";