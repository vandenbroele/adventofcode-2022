Console.WriteLine(File.ReadAllLines("input.txt")
    .Select(_ => _.Split(' ') switch
    {
        ["A", "X"] => 3 + 1,
        ["B", "Y"] => 3 + 2,
        ["C", "Z"] => 3 + 3,
        ["A", "Y"] => 6 + 2,
        ["B", "Z"] => 6 + 3,
        ["C", "X"] => 6 + 1,
        ["A", "Z"] => 0 + 3,
        ["B", "X"] => 0 + 1,
        ["C", "Y"] => 0 + 2,
        _ => 0    
    }).Sum());

Console.WriteLine(File.ReadAllLines("input.txt")
    .Select(_ => _.Split(' ') switch
    {
        ["A", "X"] => 0 + 3,
        ["B", "Y"] => 3 + 2,
        ["C", "Z"] => 6 + 1,
        ["A", "Y"] => 3 + 1,
        ["B", "Z"] => 6 + 3,
        ["C", "X"] => 0 + 2,
        ["A", "Z"] => 6 + 2,
        ["B", "X"] => 0 + 1,
        ["C", "Y"] => 3 + 3,
        _ => 0
    }).Sum());
