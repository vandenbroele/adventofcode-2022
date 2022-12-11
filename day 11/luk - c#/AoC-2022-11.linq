<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	Run(sample).Dump("sample");
	Run(input).Dump("result");
}

public object Run(string input)
{
	// PART 1
	var monkeys = input.Split("\r\n\r\n").Select(Monkey.Parse).ToArray();
		
	for (int round = 0; round < 20; round++)
	{
		foreach (var monkey in monkeys) monkey.TakeTurn(monkeys, null);
	}

	string.Join("\r\n", monkeys.Select(m => $"{m.Name} inspected items {m.InspectionLevel} times.")).Dump("Part 1");
	
	var monkeyBusiness = monkeys
		.OrderByDescending(m => m.InspectionLevel)
		.Take(2)
		.Aggregate(1L, (a, b) => a * b.InspectionLevel);

	// PART 2
	monkeys = input.Split("\r\n\r\n").Select(Monkey.Parse).ToArray();
	var mod = monkeys.Aggregate(1, (a, b) => a * b.TestValue);

	for (int round = 0; round < 10000; round++)
	{		
		foreach (var monkey in monkeys) monkey.TakeTurn(monkeys, mod);
	}

	string.Join("\r\n", monkeys.Select(m => $"{m.Name} inspected items {m.InspectionLevel} times.")).Dump("Part 2");

	var monkeyBusiness2 = monkeys
		.OrderByDescending(m => m.InspectionLevel)
		.Take(2)
		.Aggregate(1L, (a, b) => a * b.InspectionLevel);

	return new
	{
		monkeyBusiness,
		monkeyBusiness2
	};
}

public class Monkey
{
	public string Name { get; private set; }
	public List<long> Items { get; private set; }
	public string[] Operation { get; private set; }

	public int TestValue { get; private set; }
	public int TrueTest { get; private set; }
	public int FalseTest { get; private set; }

	public long InspectionLevel { get; private set; }

	public void TakeTurn(Monkey[] monkeys, int? mod)
	{		
		for (int i = 0; i < Items.Count; i++)
		{
			InspectionLevel++;
			Items[i] = CalcWorryLevel(Items[i], mod);

			if ((Items[i] % TestValue) == 0)
				monkeys[TrueTest].Items.Add(Items[i]);
			else
				monkeys[FalseTest].Items.Add(Items[i]);
		}
		Items.Clear();
	}

	private long CalcWorryLevel(long value, int? mod)
	{
		var a = Operation[0] == "old" ? value : long.Parse(Operation[0]);
		var b = Operation[2] == "old" ? value : long.Parse(Operation[2]);

		var result = Operation[1] switch
		{
			"*" => a * b,
			"+" => a + b,
			_ => throw new InvalidOperationException(),
		};
		
		return mod.HasValue ? result % mod.Value : result / 3;
	}


	public static Monkey Parse(string value)
	{
		var split = value.Split("\r\n");
		return new Monkey
		{
			Name = split[0],
			Items = split[1].Split("Starting items: ")[1].Split(", ").Select(long.Parse).ToList(),
			Operation = split[2].Split("Operation: new = ")[1].Split(' '),
			TestValue = int.Parse(split[3].Split("Test: divisible by ")[1]),
			TrueTest = int.Parse(split[4].Split("If true: throw to monkey ")[1]),
			FalseTest = int.Parse(split[5].Split("If false: throw to monkey ")[1])
		};
	}
}

public static string sample = @"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1";


public static string input = @"Monkey 0:
  Starting items: 61
  Operation: new = old * 11
  Test: divisible by 5
    If true: throw to monkey 7
    If false: throw to monkey 4

Monkey 1:
  Starting items: 76, 92, 53, 93, 79, 86, 81
  Operation: new = old + 4
  Test: divisible by 2
    If true: throw to monkey 2
    If false: throw to monkey 6

Monkey 2:
  Starting items: 91, 99
  Operation: new = old * 19
  Test: divisible by 13
    If true: throw to monkey 5
    If false: throw to monkey 0

Monkey 3:
  Starting items: 58, 67, 66
  Operation: new = old * old
  Test: divisible by 7
    If true: throw to monkey 6
    If false: throw to monkey 1

Monkey 4:
  Starting items: 94, 54, 62, 73
  Operation: new = old + 1
  Test: divisible by 19
    If true: throw to monkey 3
    If false: throw to monkey 7

Monkey 5:
  Starting items: 59, 95, 51, 58, 58
  Operation: new = old + 3
  Test: divisible by 11
    If true: throw to monkey 0
    If false: throw to monkey 4

Monkey 6:
  Starting items: 87, 69, 92, 56, 91, 93, 88, 73
  Operation: new = old + 8
  Test: divisible by 3
    If true: throw to monkey 5
    If false: throw to monkey 2

Monkey 7:
  Starting items: 71, 57, 86, 67, 96, 95
  Operation: new = old + 7
  Test: divisible by 17
    If true: throw to monkey 3
    If false: throw to monkey 1";