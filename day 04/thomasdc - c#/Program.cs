var sections = (
    from line in File.ReadAllLines("input.txt")
    let split = line.Split(',')
    let left = split[0].Split('-')
    let leftLower = int.Parse(left[0])
    let leftUpper = int.Parse(left[1])
    let right = split[1].Split('-')
    let rightLower = int.Parse(right[0])
    let rightUpper = int.Parse(right[1])
    select (left: (lower: leftLower, upper: leftUpper), right: (lower: rightLower, upper: rightUpper))).ToArray();

Console.WriteLine((
        from s in sections
        where (s.left.lower <= s.right.lower && s.left.upper >= s.right.upper) ||
              (s.right.lower <= s.left.lower && s.right.upper >= s.left.upper)
        select s)
    .Count());

Console.WriteLine((
        from s in sections
        where (s.left.upper >= s.right.lower && s.left.lower <= s.right.upper) ||
              (s.right.upper >= s.left.lower && s.right.lower <= s.left.upper)
        select s)
    .Count());