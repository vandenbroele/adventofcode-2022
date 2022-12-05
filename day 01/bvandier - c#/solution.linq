<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{
	var inputFile = @"C:\Personal\adventofcode2022\01\input.txt";
	var values = File.ReadAllLines(inputFile);

	var counts = new List<int>();
	var sum = 0;

	for (var i = 0; i < values.Length; i++)
	{
		var value = values[i];

		if (string.IsNullOrWhiteSpace(value))
		{
			counts.Add(sum);
			sum = 0;
		}
		else
		{
			sum += int.Parse(value);
		}
	}

	counts.OrderByDescending(x => x).First().Dump();
	counts.OrderByDescending(x => x).Take(3).Sum().Dump();
}

// You can define other methods, fields, classes and namespaces here