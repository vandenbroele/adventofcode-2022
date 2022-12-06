<Query Kind="Statements" />

var input = File.ReadAllText(Path.Combine(Util.CurrentQueryPath, "..", "day4-input.txt"));

IEnumerable<int> GetRange(int from, int to)
 => Enumerable.Range(from, to - from + 1);

bool OverlapsCompletely(IEnumerable<int> first, IEnumerable<int> second)
{
	return second.Intersect(first).Count() == second.Count();
}

bool OverlapsAtAll(IEnumerable<int> first, IEnumerable<int> second)
{
	return second.Intersect(first).Count() > 0;
}

// ---

var pairs = input
	.Split("\n", StringSplitOptions.RemoveEmptyEntries)
	.Select(x => x.Split(","))
	.Select(x => new
	{
		elf1 = x[0].Split("-"),
		elf2 = x[1].Split("-")
	})
	.Select(x => new
	{
		elf1range = GetRange(int.Parse(x.elf1[0]), int.Parse(x.elf1[1])),
		elf2range = GetRange(int.Parse(x.elf2[0]), int.Parse(x.elf2[1])),
	});
	
// ---
	
var answer1 = pairs
	.Where(x => OverlapsCompletely(x.elf1range, x.elf2range)
			|| OverlapsCompletely(x.elf2range, x.elf1range))
	.Count().Dump("Answer1");
	
// ---
			
var answer2 = pairs			
	.Where(x => OverlapsAtAll(x.elf1range, x.elf2range))
	.Count().Dump("Answer2");


