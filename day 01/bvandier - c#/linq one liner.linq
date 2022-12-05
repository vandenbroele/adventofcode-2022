<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{

	File.ReadAllLines(@"C:\Personal\adventofcode2022\01\input.txt").Aggregate(new List<int> { 0 }, (memo, line) =>
	{
		if (!string.IsNullOrWhiteSpace(line))
		{
			memo[memo.Count - 1] += int.Parse(line);
		}
		else
		{
			memo.Add(0);
		}

		return memo;
	})
	.OrderByDescending(x => x)
	.Take(3).Dump()
	.Sum().Dump();

	//string.Join(",", File.ReadAllLines(@"C:\Personal\adventofcode2022\01\input.txt").Select(v => string.IsNullOrWhiteSpace(v) ? " " : v))
	//	.Split(" ")
	//	.Select(x => x.Split(",", StringSplitOptions.RemoveEmptyEntries)
	//	.Sum(y => Int32.Parse(y)))
	//	.OrderByDescending(x => x)
	//	.Take(3).Dump()
	//	.Sum().Dump();
}