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
        if (name == "humn")
        {
            monkeys.Add(name, new HumanMonkey(number));
        }
        else
        {
            monkeys.Add(name, new NumberMonkey(number));
        }
    }
    else
    {
        string[] operands = operation.Split(' ');
        MathMonkey.Operation math = operands[1] switch
        {
            "+" => MathMonkey.Operation.Add,
            "-" => MathMonkey.Operation.Subtract,
            "*" => MathMonkey.Operation.Multiply,
            "/" => MathMonkey.Operation.Divide,
            _ => throw new Exception()
        };
        FetchMonkey left = new FetchMonkey(operands[0], GetMonkey);
        FetchMonkey right = new FetchMonkey(operands[2], GetMonkey);


        if (name == "root")
        {
            monkeys.Add(name, new RootMonkey(math, left, right));
        }
        else
        {
            monkeys.Add(name, new MathMonkey(math, left, right));
        }
    }
}

IMonkey GetMonkey(string name) => monkeys[name];

IMonkey rootMonkey = GetMonkey("root");
long result = rootMonkey.GetResult();
Console.WriteLine(result);

rootMonkey.Reverse(-1);
HumanMonkey human = GetMonkey("humn") as HumanMonkey;
Console.WriteLine(human.Result);

internal record Input(string Path);

public interface IMonkey
{
    public long GetResult();
    public bool HasHuman();
    public void Reverse(long number);
}

public class HumanMonkey : IMonkey
{
    private readonly long initialNumber;
    public long Result { get; private set; } = -1;

    public HumanMonkey(long initialNumber)
    {
        this.initialNumber = initialNumber;
    }

    public long GetResult() => initialNumber;

    public bool HasHuman() => true;

    public void Reverse(long number) => Result = number;
}

public class RootMonkey : IMonkey
{
    private readonly IMonkey left;
    private readonly IMonkey right;

    private MathMonkey mathMonkey;

    public RootMonkey(MathMonkey.Operation operation, IMonkey left, IMonkey right)
    {
        this.left = left;
        this.right = right;

        mathMonkey = new MathMonkey(operation, left, right);
    }

    public long GetResult() => mathMonkey.GetResult();

    public bool HasHuman() => true;

    public void Reverse(long number)
    {
        if (left.HasHuman())
        {
            long desiredResult = right.GetResult();
            left.Reverse(desiredResult);
        }
        else
        {
            long desiredResult = left.GetResult();
            right.Reverse(desiredResult);
        }
    }
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
    public bool HasHuman() => getter(name).HasHuman();

    public void Reverse(long number) => getter(name).Reverse(number);
}

public class NumberMonkey : IMonkey
{
    private readonly long number;

    public NumberMonkey(long number)
    {
        this.number = number;
    }

    public long GetResult() => number;
    public bool HasHuman() => false;

    public void Reverse(long _)
    {
    }
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

    public bool HasHuman() => left.HasHuman() || right.HasHuman();

    public void Reverse(long number)
    {
        if (left.HasHuman())
        {
            long rightResult = right.GetResult();

            left.Reverse(operation switch
            {
                Operation.Add => number - rightResult,
                Operation.Subtract => number + rightResult,
                Operation.Multiply => number / rightResult,
                Operation.Divide => number * rightResult,
                _ => throw new ArgumentOutOfRangeException()
            });
        }
        else
        {
            long leftResult = left.GetResult();

            right.Reverse(operation switch
            {
                Operation.Add => number - leftResult,
                Operation.Subtract => leftResult - number,
                Operation.Multiply => number / leftResult,
                Operation.Divide => leftResult / number,
                _ => throw new ArgumentOutOfRangeException()
            });
        }
    }

    public enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
}