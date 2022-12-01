Console.WriteLine(File.ReadAllText("input.txt")
    .Split("\r\n\r\n")
    .Select(_ => _.Split("\r\n").Select(int.Parse).Sum())
    .Max());

Console.WriteLine(File.ReadAllText("input.txt")
    .Split("\r\n\r\n")
    .Select(_ => _.Split("\r\n").Select(int.Parse).Sum())
    .OrderBy(_ => _)
    .TakeLast(3)
    .Sum());
