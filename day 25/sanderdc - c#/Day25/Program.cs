using System.Numerics;

Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

IList<string> inputLines = File.ReadLines(inputs[1].Path).ToArray();

BigInteger sum = 0;
foreach (BigInteger bigInteger in inputLines
             .Select(Snafu.Parse)
             .Select(s => s.ToBigInt()))
{
    sum += bigInteger;
}

Console.WriteLine($"sum is {sum}");
Console.WriteLine($"part1: {Snafu.FromBigInt(sum)}");

internal record Input(string Path);