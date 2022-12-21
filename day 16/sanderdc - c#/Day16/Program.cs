using System.Text.RegularExpressions;

Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

// Input
Input input = inputs[0];
List<string> inputLines = File.ReadLines(input.Path).ToList();

// Info
Dictionary<string, int> nameToId = new();
Dictionary<int, string> idToName = new();
Dictionary<int, Valve> valves = ParseValves(inputLines);

// minute / valveId / bitmask => score
Dictionary<int, Dictionary<int, Dictionary<long, int>>> memo = new();
// minute / valveId / valveId2 / bitmask => score
Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<long, int>>>> memo2 = new();
Dictionary<int, bool> openValves = new();
Dictionary<(int, int), int> distances = PrecomputeDistances(valves);

Valve startValve = valves[nameToId["AA"]];

Console.WriteLine($"part 1: {Solve(startValve.Id, 30, 0)}");
Console.WriteLine($"part 2: {Solve2(startValve.Id, startValve.Id, 26, 0)}");


void OpenValve(int valveId) => openValves[valveId] = true;

void CloseValve(int valveId) => openValves[valveId] = false;

bool IsValveOpen(int valveId) => openValves.ContainsKey(valveId) && openValves[valveId];


int GetMemo(int remainingMinutes, int valveIndex, long bitMask)
{
    if (!memo.ContainsKey(remainingMinutes)) return -1;
    if (!memo[remainingMinutes].ContainsKey(valveIndex)) return -1;
    if (!memo[remainingMinutes][valveIndex].ContainsKey(bitMask)) return -1;

    return memo[remainingMinutes][valveIndex][bitMask];
}

void SetMemo(int remainingMinutes, int valveIndex, long bitMask, int value)
{
    if (!memo.ContainsKey(remainingMinutes))
        memo[remainingMinutes] = new Dictionary<int, Dictionary<long, int>>();

    if (!memo[remainingMinutes].ContainsKey(valveIndex))
        memo[remainingMinutes][valveIndex] = new Dictionary<long, int>();

    memo[remainingMinutes][valveIndex][bitMask] = value;
}

int GetMemo2(int remainingMinutes, int valveIndex, int valveIndex2, long bitMask)
{
    if (!memo2.ContainsKey(remainingMinutes)) return -1;
    if (!memo2[remainingMinutes].ContainsKey(valveIndex)) return -1;
    if (!memo2[remainingMinutes][valveIndex].ContainsKey(valveIndex2)) return -1;
    if (!memo2[remainingMinutes][valveIndex][valveIndex2].ContainsKey(bitMask)) return -1;

    return memo2[remainingMinutes][valveIndex][valveIndex2][bitMask];
}

void SetMemo2(int remainingMinutes, int valveIndex, int valveIndex2, long bitMask, int value)
{
    if (!memo2.ContainsKey(remainingMinutes))
        memo2[remainingMinutes] = new Dictionary<int, Dictionary<int, Dictionary<long, int>>>();

    if (!memo2[remainingMinutes].ContainsKey(valveIndex))
        memo2[remainingMinutes][valveIndex] = new Dictionary<int, Dictionary<long, int>>();

    if (!memo2[remainingMinutes][valveIndex].ContainsKey(valveIndex2))
        memo2[remainingMinutes][valveIndex][valveIndex2] = new Dictionary<long, int>();

    memo2[remainingMinutes][valveIndex][valveIndex2][bitMask] = value;
}

int Solve(int currentValveId, int remainingMinutes, long bitMask)
{
    if (remainingMinutes == 0)
        return 0;

    int memoValue = GetMemo(remainingMinutes, currentValveId, bitMask);
    if (memoValue != -1)
    {
        return memoValue;
    }

    int maxFlow = 0;

    Valve currentValve = valves[currentValveId];
    bool canOpenValve = !IsValveOpen(currentValveId);
    int currentValveFlowRate = (remainingMinutes - 1) * currentValve.FlowRate;

    foreach ((int nextValveId, long bitmask, int flow) in Options())
    {
        maxFlow = Math.Max(maxFlow, flow + Solve(nextValveId, remainingMinutes - 1, bitmask));
    }

    IEnumerable<(int currentValveId, long bitmask, int flow )> Options()
    {
        bool shouldOpenValve = currentValve.FlowRate != 0;
        bool hasTimeToOpenValve = 2 <= remainingMinutes;

        if (hasTimeToOpenValve && canOpenValve && shouldOpenValve)
        {
            OpenValve(currentValveId);
            yield return (currentValveId, bitMask + currentValve.BitMask, currentValveFlowRate);
            CloseValve(currentValveId);
        }

        // Return connections
        foreach (int connection in currentValve.Connections)
        {
            yield return (connection, bitMask, 0);
        }
    }

    SetMemo(remainingMinutes, currentValveId, bitMask, maxFlow);
    return maxFlow;
}


int Solve2(int humanValveId, int elephantValveId, int remainingMinutes, long bitMask)
{
    if (remainingMinutes == 0)
        return 0;

    int memoValue = GetMemo2(remainingMinutes, humanValveId, elephantValveId, bitMask);
    if (memoValue != -1)
    {
        return memoValue;
    }

    int maxFlow = 0;

    foreach ((int nextHumanValveId, long bitmask, int flow) in HumanOptions())
    {
        foreach ((int nextElephantValveId, long updatedBitmask, int updatedFlow)
                 in ElephantOptions(humanValveId, bitmask, flow))
        {
            maxFlow = Math.Max(maxFlow, updatedFlow + Solve2(
                nextHumanValveId,
                nextElephantValveId,
                remainingMinutes - 1,
                updatedBitmask));
        }
    }

    IEnumerable<(int nextElephantValveId, long updatedBitmask, int updatedFlow)> ElephantOptions(
        int humanId,
        long tempBitmask,
        int humanFlow)
    {
        Valve currentElephantValve = valves[elephantValveId];

        int currentValveFlowRate = (remainingMinutes - 1) * currentElephantValve.FlowRate;
        bool shouldOpenValve = currentElephantValve.FlowRate != 0;
        bool hasTimeToOpenValve = 2 <= remainingMinutes;
        bool canOpenValve = !IsValveOpen(elephantValveId);

        if (hasTimeToOpenValve && canOpenValve && shouldOpenValve)
        {
            OpenValve(elephantValveId);
            yield return (
                elephantValveId,
                tempBitmask + currentElephantValve.BitMask,
                humanFlow + currentValveFlowRate);
            CloseValve(elephantValveId);
        }
        
        foreach (int connection in currentElephantValve.Connections)
        {
            yield return (connection, tempBitmask, humanFlow);
        }
    }


    IEnumerable<(int nextHumanValveId, long bitmask, int flow)> HumanOptions()
    {
        Valve currentValve = valves[humanValveId];
        int currentValveFlowRate = (remainingMinutes - 1) * currentValve.FlowRate;
        bool shouldOpenValve = currentValve.FlowRate != 0;
        bool hasTimeToOpenValve = 2 <= remainingMinutes;
        bool canOpenValve = !IsValveOpen(humanValveId);

        if (hasTimeToOpenValve && canOpenValve && shouldOpenValve)
        {
            OpenValve(humanValveId);
            yield return (humanValveId, bitMask + currentValve.BitMask, currentValveFlowRate);
            CloseValve(humanValveId);
        }

        // Return connections
        foreach (int connection in currentValve.Connections)
        {
            yield return (connection, bitMask, 0);
        }
    }

    SetMemo2(remainingMinutes, humanValveId, elephantValveId, bitMask, maxFlow);
    return maxFlow;
}

Dictionary<(int, int), int> PrecomputeDistances(Dictionary<int, Valve> inputValves)
{
    Dictionary<(int, int), int> result = new();

    foreach (int from in inputValves.Keys)
    {
        foreach (int to in inputValves.Keys)
        {
            if (from == to)
                continue;

            result.Add((from, to), GetDistance(inputValves, from, to));
        }
    }

    return result;
}

int GetDistance(Dictionary<int, Valve> graph, int from, int to)
{
    List<int> candidates = new();
    Dictionary<int, int> valveDistances = new(graph.Count);

    foreach (int graphKey in graph.Keys)
    {
        valveDistances[graphKey] = int.MaxValue;
    }

    candidates.Add(from);
    valveDistances[from] = 0;

    while (candidates.Count > 0)
    {
        int currentId = candidates[^1];
        candidates.RemoveAt(candidates.Count - 1);

        if (currentId == to)
        {
            return valveDistances[currentId];
        }

        int currentDistance = valveDistances[currentId];
        Valve current = graph[currentId];

        foreach (int nextConnection in current.Connections)
        {
            if (valveDistances[nextConnection] < currentDistance + 1)
                continue;

            valveDistances[nextConnection] = currentDistance + 1;
            candidates.Add(nextConnection);
        }
    }

    return -1;
}

Dictionary<int, Valve> ParseValves(List<string> list)
{
    Dictionary<int, Valve> result = new();

    int counter = 0;
    long bitMask = 1;

    foreach (string inputLine in list)
    {
        string name = inputLine[6..8];
        int valveId = ++counter;

        nameToId[name] = valveId;
        idToName[valveId] = name;
    }

    foreach (string inputLine in list)
    {
        string name = inputLine[6..8];
        int valveId = nameToId[name];

        int equalsIndex = inputLine.IndexOf('=');
        int semiColonIndex = inputLine.IndexOf(';');
        string flowRateString = inputLine.Substring(equalsIndex + 1, semiColonIndex - equalsIndex - 1);
        int flowRate = int.Parse(flowRateString);

        Match m = Regex.Match(inputLine, "valves*.*");

        int[] connections = m.Value[(m.Value.IndexOf(' ') + 1)..]
            .Split(", ")
            .Select(s => nameToId[s])
            .ToArray();

        Valve valve = new(valveId, bitMask, flowRate, connections);
        bitMask *= 2;

        result.Add(valveId, valve);
    }

    return result;
}

internal record Input(string Path);

internal record Valve(int Id, long BitMask, int FlowRate, int[] Connections);