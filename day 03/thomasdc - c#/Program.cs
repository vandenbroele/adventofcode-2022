Console.WriteLine(
    (
        from line in File.ReadAllLines("input.txt")
        let firstHalf = line[..(line.Length / 2)]
        let secondHalf = line[^(line.Length / 2)..]
        select firstHalf.Intersect(secondHalf))
    .SelectMany(_ => _)
    .Select(_ => (int) _)
    .Sum(c => c switch
    {
        >= 'a' => c - 'a' + 1,
        >= 'A' => c - 'A' + 27,
        _ => 0
    }));

Console.WriteLine(File.ReadAllLines("input.txt").Chunk(3)
    .Sum(group => (
        from g in
            from line in @group
            from @char in line.Distinct()
            let intChar = (int) @char
            group intChar by intChar
        where g.Count() == 3
        select g.Key switch
        {
            >= 'a' => g.Key - 'a' + 1,
            >= 'A' => g.Key - 'A' + 27,
            _ => 0
        }).Sum()));