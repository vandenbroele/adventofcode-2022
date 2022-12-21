using System.Text.RegularExpressions;

Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

Input input = inputs[0];
List<string> inputLines = File.ReadLines(input.Path).ToList();


Dictionary<int, string> idLookup = new();
Dictionary<int, Valve> valves = ParseValves(inputLines);
Dictionary<(int, int), int> distances = PrecomputeDistances(valves);


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

    foreach (string inputLine in list)
    {
        string name = inputLine[6..8];
        int valveNameHash = name.GetHashCode();

        int equalsIndex = inputLine.IndexOf('=');
        int semiColonIndex = inputLine.IndexOf(';');
        string flowRateString = inputLine.Substring(equalsIndex + 1, semiColonIndex - equalsIndex - 1);
        int flowRate = int.Parse(flowRateString);

        Match m = Regex.Match(inputLine, "valves*.*");

        int[] connections = m.Value[(m.Value.IndexOf(' ') + 1)..]
            .Split(", ")
            .Select(s => s.GetHashCode())
            .ToArray();

        Valve valve = new(flowRate, connections);


        idLookup.Add(valveNameHash, name);
        result.Add(valveNameHash, valve);
    }

    return result;
}

internal record Input(string Path);

internal record Valve(int FlowRate, int[] Connections);