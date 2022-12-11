string[] paths = { "./input.txt", "./testInput.txt" };

IEnumerable<string> input = File.ReadLines(paths[0]);

List<Monkey> monkeys = input
    .Chunk(7)
    .Select(Monkey.Parse)
    .ToList();

int magicModulo = 1;
foreach (Monkey monkey in monkeys)
{
    monkey.ThrowTo += ThrowTo;
    magicModulo *= monkey.Test;
}

for (int i = 0; i < 10000; i++)
{
    foreach (Monkey monkey in monkeys)
    {
        monkey.DoRound(magicModulo);
    }
}


void LogMonkeyInventories()
{
    foreach (Monkey monkey in monkeys)
    {
        Console.WriteLine($"{monkey.Id}: {string.Join(", ", monkey.Items)}");
    }

    Console.WriteLine();
}

foreach (Monkey monkey in monkeys)
{
    Console.WriteLine($"{monkey.Id} inspected {monkey.Inspections}");
}

long monkeyBusiness = monkeys
    .Select(m => m.Inspections)
    .OrderByDescending(i => i)
    .Take(2)
    .Aggregate((i, i1) => i * i1);

Console.WriteLine(monkeyBusiness);

void ThrowTo(int target, long item)
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
                _ => new MultiplyNumber(long.Parse(argument))
            },
            '+' => new AddNumber(long.Parse(argument)),
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
    public long Inspections { get; private set; }
    public int Test { get; }

    public IEnumerable<long> Items => items;

    private readonly List<long> items;
    private readonly IOperation operation;
    private readonly int trueTarget;
    private readonly int falseTarget;

    public event Action<int, long> ThrowTo;

    private Monkey(int id, IEnumerable<int> startingItems, IOperation operation, int divisibleBy,
        int trueTarget,
        int falseTarget)
    {
        Id = id;
        items = new List<long>(startingItems.Select(i=>(long)i));
        this.operation = operation;
        Test = divisibleBy;
        this.trueTarget = trueTarget;
        this.falseTarget = falseTarget;
    }

    public void DoRound(int modulo)
    {
        foreach (long t in items)
        {
            Inspections++;

            long item = operation.Evaluate(t);

            // Lower worry (part 1)
            // item /= 3;

            // Manage worry (part 2)
            item %= (long)modulo;

            int target = item % Test == 0
                ? trueTarget
                : falseTarget;

            ThrowTo(target, item);
        }

        items.Clear();
    }

    public void Catch(long item)
    {
        items.Add(item);
    }
}

public interface IOperation
{
    long Evaluate(long input);
}

public class MultiplySelf : IOperation
{
    public long Evaluate(long input)
    {
        return input * input;
    }
}

public class AddNumber : IOperation
{
    private readonly long number;

    public AddNumber(long number)
    {
        this.number = number;
    }

    public long Evaluate(long input)
    {
        return input + number;
    }
}

public class MultiplyNumber : IOperation
{
    private readonly long number;

    public MultiplyNumber(long number)
    {
        this.number = number;
    }

    public long Evaluate(long input)
    {
        return input * number;
    }
}