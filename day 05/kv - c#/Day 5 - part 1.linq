<Query Kind="Statements" />

var input = File.ReadAllText(Path.Combine(Util.CurrentQueryPath, "..", "day5-input.txt"));

var stacksText = input.Split("\n\n")[0].Split("\n");
var movesText = input.Split("\n\n")[1].Split("\n");

var stacks = ReadStacks(stacksText.Reverse().ToArray());
var moves = ReadMoves(movesText);

DoMoves(stacks, moves);

var answer1 = new String(stacks.Values.Select(x => x.Peek()).ToArray()).Dump("Answer 1");

//----

void DoMoves(Dictionary<int, Stack<char>> stacks, List<(int crates, int from, int to)> moves)
{
	foreach (var move in moves)
	{
		for (var i = 0; i < move.crates; i++)
			stacks[move.to].Push(stacks[move.from].Pop());
	}
}

Dictionary<int, Stack<char>> ReadStacks(string[] stacksText)
{
	var result = new Dictionary<int, Stack<char>>();
	
	foreach(var i in stacksText[0].Split(" ", StringSplitOptions.RemoveEmptyEntries))
	{
		result.Add(int.Parse(i), new Stack<char>());
	}
	for (var i = 1; i < stacksText.Length; i++)
	{
		var line = stacksText[i];
		
		foreach (var key in result.Keys)
		{
			var crate = line.Substring(1 + ((key-1) * 4), 1);//.Dump("Stack " + key);
			if (crate[0] == ' ') continue;
			result[key].Push(crate[0]);
		}
	}
			
	return result;
}

List<(int crates, int from, int to)> ReadMoves(string[] movesText)
{
	return movesText
		.Where(x => x.StartsWith("move"))
		.Select(x => x.Substring(5).Split(new[] { "from", "to" }, 3, StringSplitOptions.RemoveEmptyEntries))
		.Select(x => (int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2])))
		.ToList();
}