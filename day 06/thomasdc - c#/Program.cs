var stream = File.ReadAllText("input.txt");
const int window = 14;
Console.WriteLine((
    from x in Enumerable.Range(window, stream.Length)
    let sub = stream[(x - window)..x]
    where sub.Distinct().Count() == window
    select (x, sub)).First());