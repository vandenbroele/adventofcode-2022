using System.Text.RegularExpressions;

Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

// Input
Input input = inputs[1];
List<string> inputLines = File.ReadLines(input.Path).ToList();

// Info
Dictionary<string, int> nameToId = new();
Dictionary<int, string> idToName = new();
Dictionary<int, Valve> valves = ParseValves(inputLines);

// minute / valveId / bitmask => score
Dictionary<int, Dictionary<int, Dictionary<long, int>>> memo = new();
Dictionary<int, bool> openValves = new();
Dictionary<(int, int), int> distances = PrecomputeDistances(valves);

Valve startValve = valves[nameToId["AA"]];

Console.WriteLine(Solve(startValve.Id, 30, 0));


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

    if (canOpenValve)
    {
        maxFlow = currentValveFlowRate;
    }

    foreach (int connection in currentValve.Connections)
    {
        maxFlow = Math.Max(maxFlow, Solve(connection, remainingMinutes - 1, bitMask));

        bool shouldOpenValve = currentValve.FlowRate != 0;
        bool hasTimeToOpenValve = 2 <= remainingMinutes;

        if (!hasTimeToOpenValve || !canOpenValve || !shouldOpenValve)
            continue;

        OpenValve(currentValveId);

        int connectionSolve = Solve(
            connection,
            remainingMinutes - 2,
            bitMask + currentValve.BitMask);

        maxFlow = Math.Max(maxFlow, currentValveFlowRate + connectionSolve);

        CloseValve(currentValveId);
    }

    SetMemo(remainingMinutes, currentValveId, bitMask, maxFlow);
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