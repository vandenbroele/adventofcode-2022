Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

Input input = inputs[1];
List<string> inputLines = File.ReadLines(input.Path).ToList();

Dictionary<string, IMonkey> monkeys = new();

foreach (string inputLine in inputLines)
{
    string name = inputLine[..inputLine.IndexOf(':')];
    string operation = inputLine[6..];


    if (int.TryParse(operation, out int number))
    {
        monkeys.Add(name, new NumberMonkey(number));
    }
    else
    {
        string[] operands = operation.Split(' ');
        monkeys.Add(name, new MathMonkey(
            operands[1] switch
            {
                "+" => MathMonkey.Operation.Add,
                "-" => MathMonkey.Operation.Subtract,
                "*" => MathMonkey.Operation.Multiply,
                "/" => MathMonkey.Operation.Divide,
                _ => throw new Exception()
            },
            new FetchMonkey(operands[0], GetMonkey),
            new FetchMonkey(operands[2], GetMonkey)));
    }
}

IMonkey GetMonkey(string name) => monkeys[name];

long result = GetMonkey("root").GetResult();
Console.WriteLine(result);

internal record Input(string Path);


public interface IMonkey
{
    public long GetResult();
}

public class FetchMonkey : IMonkey
{
    private readonly string name;
    private readonly Func<string, IMonkey> getter;

    public FetchMonkey(string name, Func<string, IMonkey> getter)
    {
        this.getter = getter;
        this.name = name;
    }

    public long GetResult() => getter(name).GetResult();
}

public class NumberMonkey : IMonkey
{
    private readonly long number;

    public NumberMonkey(long number)
    {
        this.number = number;
    }

    public long GetResult() => number;
}

public class MathMonkey : IMonkey
{
    private readonly Operation operation;
    private readonly IMonkey left;
    private readonly IMonkey right;

    public MathMonkey(Operation operation, IMonkey left, IMonkey right)
    {
        this.operation = operation;
        this.left = left;
        this.right = right;
    }


    public long GetResult()
    {
        return operation switch
        {
            Operation.Add => left.GetResult() + right.GetResult(),
            Operation.Subtract => left.GetResult() - right.GetResult(),
            Operation.Multiply => left.GetResult() * right.GetResult(),
            Operation.Divide => left.GetResult() / right.GetResult(),
            _ => throw new Exception()
        };
    }

    public enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
}