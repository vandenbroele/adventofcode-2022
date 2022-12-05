using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");
var height = lines
    .Select((s, i) => (s, i))
    .First(_ => _.s.StartsWith(" 1")).i;
var stacks = Enumerable.Range(0, lines[height].Length / 4 + 1)
    .Select(_ => new Stack<char>())
    .ToArray();

for (var y = height - 1; y >= 0; y--)
{
    foreach (var s in Enumerable.Range(0, stacks.Length))
    {
        var item = lines[y][4 * s + 1];
        if (item != ' ')
        {
            stacks[s].Push(item);
        }
    }
}

var regex = new Regex("move (?<count>\\d+) from (?<from>\\d+) to (?<to>\\d+)", RegexOptions.Compiled);
foreach (var instruction in lines.Skip(height + 2))
{
    var match = regex.Match(instruction);
    var count = int.Parse(match.Groups["count"].Value);
    var from = int.Parse(match.Groups["from"].Value) - 1;
    var to = int.Parse(match.Groups["to"].Value) - 1;
    var sub = new List<char>();
    for (var box = 0; box < count; box++)
    {
        sub.Add(stacks[from].Pop());
    }

    sub.Reverse();
    foreach (var s in sub)
    {
        stacks[to].Push(s);
    }
}

Console.WriteLine(stacks.Select(_ => _.Peek()).ToArray());