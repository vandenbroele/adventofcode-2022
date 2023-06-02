using System.Diagnostics;
using System.Text.RegularExpressions;

Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

List<Blueprint> blueprints = File.ReadLines(inputs[1].Path).Select(ParseBlueprint).ToList();

// Part 1
const int part1Minutes = 24;
int part1Total = 0;
foreach (Blueprint blueprint in blueprints)
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    State state = new(
        0,
        new Resources(
            1, 0, 0, 0,
            0, 0, 0, 0));

    int brol = 0;
    int maxGeodes = SolveGeodes(
        state,
        blueprint,
        part1Minutes,
        new Dictionary<Resources, (int result, int minutes)>(),
        ref brol,
        true, true, true);
    Console.WriteLine(
        $"blueprint {blueprint.Id}: {maxGeodes} geodes found score is {maxGeodes * blueprint.Id} (took {stopwatch.Elapsed})");

    part1Total += blueprint.Id * maxGeodes;
}

Console.WriteLine($"part 1: {part1Total}");

// Part 2
const int part2Minutes = 32;
int total = 1;
foreach (Blueprint blueprint in blueprints.Take(3))
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    State state = new(
        0,
        new Resources(
            1, 0, 0, 0,
            0, 0, 0, 0));

    int brol = 0;
    int maxGeodes = SolveGeodes(
        state,
        blueprint,
        part2Minutes,
        new Dictionary<Resources, (int result, int minutes)>(),
        ref brol,
        true, true, true);

    total *= maxGeodes;
    Console.WriteLine(
        $"blueprint {blueprint.Id}: {maxGeodes} geodes found (took {stopwatch.Elapsed})");
}

Console.WriteLine($"part 2: {total}");


static int SolveGeodes(
    State state,
    Blueprint blueprint,
    int maxMinutes,
    IDictionary<Resources, (int result, int minutes)> memo,
    ref int maxGeodes,
    bool allowOre, bool allowClay, bool allowObsidian)
{
    if (memo.TryGetValue(state.Resources, out (int result, int minutes) cachedValue))
    {
        if (state.Minute > cachedValue.minutes)
        {
            // Console.WriteLine("returning from cache");
            return cachedValue.result;
        }
    }

    if (state.Minute >= maxMinutes)
    {
        // Console.WriteLine($"found an end result of: {state.Resources.Geodes}");
        return state.Resources.Geodes;
    }

    if (!state.CanReach(maxGeodes, maxMinutes))
    {
        return 0;
    }

    int result = 0;
    bool couldBuildOre = false;
    bool couldBuildClay = false;
    bool couldBuildObsidian = false;

    if (state.CanBuildOreBot(blueprint))
    {
        couldBuildOre = true;
        State alteredState = state.Step().BuildOreBot(blueprint);
        if (allowOre && state.MoreOreBotsRequired(blueprint))
        {
            result = int.Max(
                result,
                SolveGeodes(alteredState, blueprint, maxMinutes, memo, ref maxGeodes, true, true, true));
        }
    }

    if (state.CanBuildClayBot(blueprint))
    {
        couldBuildClay = true;
        State alteredState = state.Step().BuildClayBot(blueprint);

        if (allowClay && state.MoreClayBotsRequired(blueprint))
        {
            result = int.Max(
                result,
                SolveGeodes(alteredState, blueprint, maxMinutes, memo, ref maxGeodes, true, true, true));
        }
    }

    if (state.CanBuildObsidianBot(blueprint))
    {
        couldBuildObsidian = true;
        State alteredState = state.Step().BuildObsidianBot(blueprint);

        if (allowObsidian && state.MoreObsidianBotsRequired(blueprint))
        {
            result = int.Max(
                result,
                SolveGeodes(alteredState, blueprint, maxMinutes, memo, ref maxGeodes, true, true, true));
        }
    }

    if (state.CanBuildGeodeBot(blueprint))
    {
        State alteredState = state.Step().BuildGeodeBot(blueprint);
        result = int.Max(
            result,
            SolveGeodes(alteredState, blueprint, maxMinutes, memo, ref maxGeodes, true, true, true));
    }

    // Do nothing
    result = int.Max(
        result,
        SolveGeodes(
            state.Step(), blueprint, maxMinutes, memo, ref maxGeodes,
            !couldBuildOre, !couldBuildClay, !couldBuildObsidian));

    memo[state.Resources] = (result, state.Minute);
    // Console.WriteLine($"caching state. {memo.Count} states cached");

    maxGeodes = int.Max(maxGeodes, result);

    return result;
}


static Blueprint ParseBlueprint(string line)
{
    MatchCollection matches = Regex.Matches(line, "[0-9]+");

    if (matches.Count != 7)
        throw new Exception($"Did not find correct amount ouf numbers. Expected 7 but got {matches.Count} for line ");

    return new Blueprint(
        int.Parse(matches[0].Value),
        int.Parse(matches[1].Value),
        int.Parse(matches[2].Value),
        (int.Parse(matches[3].Value), int.Parse(matches[4].Value)),
        (int.Parse(matches[5].Value), int.Parse(matches[6].Value)));
}

internal record Input(string Path);

internal readonly record struct Blueprint(
    int Id,
    int OreBotCostOre,
    int ClayBotCostOre,
    (int Ore, int Clay) ObsidianBotCost,
    (int Ore, int Obsidian) GeodeBotCost)
{
    public int MaxOreCost =>
        int.Max(OreBotCostOre, int.Max(ClayBotCostOre, int.Max(ObsidianBotCost.Ore, GeodeBotCost.Ore)));
};


internal readonly record struct Resources(
    int OreBots, int ClayBots, int ObsidianBots, int GeodeBots,
    int Ore, int Clay, int Obsidian, int Geodes);

internal readonly record struct State(int Minute, Resources Resources)
{
    public bool CanBuildOreBot(Blueprint blueprint) => blueprint.OreBotCostOre <= Resources.Ore;
    public bool CanBuildClayBot(Blueprint blueprint) => blueprint.ClayBotCostOre <= Resources.Ore;

    public bool CanBuildObsidianBot(Blueprint blueprint) =>
        blueprint.ObsidianBotCost.Ore <= Resources.Ore && blueprint.ObsidianBotCost.Clay <= Resources.Clay;

    public bool CanBuildGeodeBot(Blueprint blueprint) =>
        blueprint.GeodeBotCost.Ore <= Resources.Ore && blueprint.GeodeBotCost.Obsidian <= Resources.Obsidian;

    public bool MoreOreBotsRequired(Blueprint blueprint) => Resources.OreBots < blueprint.MaxOreCost;
    public bool MoreClayBotsRequired(Blueprint blueprint) => Resources.ClayBots < blueprint.ObsidianBotCost.Clay;

    public bool MoreObsidianBotsRequired(Blueprint blueprint) =>
        Resources.ObsidianBots < blueprint.GeodeBotCost.Obsidian;

    public bool CanReach(int targetGeodes, int maxMinutes)
    {
        int remainingMinutes = maxMinutes - Minute;

        int maxGeodes = Resources.Geodes
            + Resources.GeodeBots * remainingMinutes
            + remainingMinutes * (remainingMinutes - 1) / 2;

        return targetGeodes <= maxGeodes;
    }

    public State Step() => new(
        Minute: Minute + 1,
        Resources: Resources with
        {
            Ore = Resources.Ore + Resources.OreBots,
            Clay = Resources.Clay + Resources.ClayBots,
            Obsidian = Resources.Obsidian + Resources.ObsidianBots,
            Geodes = Resources.Geodes + Resources.GeodeBots
        });

    public State BuildOreBot(Blueprint blueprint) => this with
    {
        Resources = Resources with
        {
            OreBots = Resources.OreBots + 1,
            Ore = Resources.Ore - blueprint.OreBotCostOre
        }
    };

    public State BuildClayBot(Blueprint blueprint) => this with
    {
        Resources = Resources with
        {
            ClayBots = Resources.ClayBots + 1,
            Ore = Resources.Ore - blueprint.ClayBotCostOre
        }
    };

    public State BuildObsidianBot(Blueprint blueprint) => this with
    {
        Resources = Resources with
        {
            ObsidianBots = Resources.ObsidianBots + 1,
            Ore = Resources.Ore - blueprint.ObsidianBotCost.Ore,
            Clay = Resources.Clay - blueprint.ObsidianBotCost.Clay
        }
    };

    public State BuildGeodeBot(Blueprint blueprint) => this with
    {
        Resources = Resources with
        {
            GeodeBots = Resources.GeodeBots + 1,
            Ore = Resources.Ore - blueprint.GeodeBotCost.Ore,
            Obsidian = Resources.Obsidian - blueprint.GeodeBotCost.Obsidian
        }
    };
};