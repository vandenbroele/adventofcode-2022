Console.WriteLine(
    (
        from line in
            from line in File.ReadAllLines("input.txt")
            let firstHalf = line[..(line.Length / 2)]
            let secondHalf = line[^(line.Length / 2)..]
            select firstHalf.Intersect(secondHalf)
        from c in line
        select c switch
        {
            >= 'a' => c - 'a' + 1,
            >= 'A' => c - 'A' + 27,
            _ => 0
        }
    ).Sum());

Console.WriteLine(File.ReadAllLines("input.txt").Chunk(3)
    .Sum(group => (
        from g in
            from line in @group
            from @char in line.Distinct()
            group @char by (int) @char
        where g.Count() == 3
        select g.Key switch
        {
            >= 'a' => g.Key - 'a' + 1,
            >= 'A' => g.Key - 'A' + 27,
            _ => 0
        }).Sum()));