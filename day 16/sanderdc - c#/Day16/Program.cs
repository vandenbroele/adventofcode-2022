Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

Input input = inputs[0];
List<string> inputLines = File.ReadLines(input.Path).ToList();


internal record Input(string Path);