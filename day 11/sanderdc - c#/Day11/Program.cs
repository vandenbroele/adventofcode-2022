string[] paths = { "./input.txt", "./testInput.txt" };

IEnumerable<string> input = File.ReadLines(paths[0]);


List<Monkey> monkeys = input
    .Chunk(7)
    .Select(Monkey.Parse)
    .ToList();

Dictionary<int, int> inspections = new();
foreach (Monkey monkey in monkeys)
{
    inspections.Add(monkey.Id, 0);
    monkey.ThrowTo += ThrowTo;
}

for (int i = 0; i < 20; i++)
{
    foreach (Monkey monkey in monkeys)
    {
        inspections[monkey.Id] += monkey.ItemCount;

        monkey.DoRound();
        
    }

    foreach (Monkey monkey in monkeys)
    {
        Console.WriteLine($"{monkey.Id}: {string.Join(", ",monkey.Items)}");
    }

    Console.WriteLine();
}

foreach (int inspectionsKey in inspections.Keys)
{
    Console.WriteLine($"{inspectionsKey} inspected {inspections[inspectionsKey]}");
}

int result = inspections
    .Select(pair => pair.Value)
    .OrderByDescending(i => i)
    .Take(2)
    .Aggregate((i, i1) => i * i1);

Console.WriteLine(result);

void ThrowTo(int target, int item)
{
    monkeys.First(m => m.Id == target).Catch(item);
}

internal class Monkey
{
    public static Monkey Parse(IEnumerable<string> input)
    {
        using IEnumerator<string> enumerator = input.GetEnumerator();
        enumerator.MoveNext();
        int id = (int)char.GetNumericValue(enumerator.Current[^2]);

        enumerator.MoveNext();
        IEnumerable<int> items = enumerator.Current
            .Split(": ")[^1]
            .Split(", ")
            .Select(int.Parse);

        enumerator.MoveNext();
        string operationInput = enumerator.Current.Split("old ")[^1];
        string argument = operationInput.Split(" ")[^1];

        IOperation operation = operationInput[0] switch
        {
            '*' => argument switch
            {
                "old" => new MultiplySelf(),
                _ => new MultiplyNumber(int.Parse(argument))
            },
            '+' => new AddNumber(int.Parse(argument)),
            _ => throw new ArgumentOutOfRangeException()
        };

        enumerator.MoveNext();
        int divisibleBy = int.Parse(enumerator.Current.Split(' ')[^1]);
        enumerator.MoveNext();
        int trueTarget = int.Parse(enumerator.Current.Split(' ')[^1]);
        enumerator.MoveNext();
        int falseTarget = int.Parse(enumerator.Current.Split(' ')[^1]);

        return new Monkey(id, items, operation, divisibleBy, trueTarget, falseTarget);
    }

    public int Id { get; }
    public int ItemCount => items.Count;
    public IEnumerable<int> Items => items;

    private readonly List<int> items;
    private readonly IOperation operation;
    private readonly int divisibleByTest;
    private readonly int trueTarget;
    private readonly int falseTarget;

    public event Action<int, int> ThrowTo;

    private Monkey(int id, IEnumerable<int> startingItems, IOperation operation, int divisibleBy,
        int trueTarget,
        int falseTarget)
    {
        Id = id;
        items = new List<int>(startingItems);
        this.operation = operation;
        divisibleByTest = divisibleBy;
        this.trueTarget = trueTarget;
        this.falseTarget = falseTarget;
    }

    public void DoRound()
    {
        foreach (int t in items)
        {
            int item = t;
            item = operation.Evaluate(item);

            // Lower worry
            item /= 3;


            int target = item % divisibleByTest == 0
                ? trueTarget
                : falseTarget;

            ThrowTo(target, item);
        }

        items.Clear();
    }

    public void Catch(int item)
    {
        items.Add(item);
    }
}

internal interface IOperation
{
    int Evaluate(int input);
}

internal class MultiplySelf : IOperation
{
    public int Evaluate(int input)
    {
        return input * input;
    }
}

internal class AddNumber : IOperation
{
    private readonly int number;

    public AddNumber(int number)
    {
        this.number = number;
    }

    public int Evaluate(int input)
    {
        return input + number;
    }
}

internal class MultiplyNumber : IOperation
{
    private readonly int number;

    public MultiplyNumber(int number)
    {
        this.number = number;
    }

    public int Evaluate(int input)
    {
        return input * number;
    }
}