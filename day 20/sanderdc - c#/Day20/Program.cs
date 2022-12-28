Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

List<string> inputLines = File.ReadLines(inputs[1].Path).ToList();

List<long> order = inputLines
    .Select(s => long.Parse(s) * 811589153)
    .ToList();
LinkedList<Thing> sequence = new(order
    .Select((value, index) => new Thing(index, value)));

// Console.WriteLine($"start {string.Join(", ", sequence.Select(t => t.Value))}");

for (int round = 0; round < 10; ++round)
{
    for (int index = 0; index < order.Count; index++)
    {
        long current = order[index];
        Move(sequence, new Thing(index, current));
        // Console.WriteLine(
        //     $"during round {round + 1}, move {index + 1}: {string.Join(", ", sequence.Select(t => t.Value))}");
    }

    // Console.WriteLine($"at round {round + 1}: {string.Join(", ", sequence.Select(t => t.Value))}");
}

(long first, long second, long third) = First(order, sequence);

Console.WriteLine($"part 2: {first + second + third}");

void Move(LinkedList<Thing> ints, Thing value)
{
    if (value.Value == 0)
        return;

    LinkedListNode<Thing>? start = ints.Find(value)!;

    LinkedListNode<Thing>? pos = start.Previous ?? ints.Last;
    ints.Remove(start);

    if (value.Value > 0)
    {
        long offset = value.Value % ints.Count;
        // offset = value.Value;
        for (long i = 0; i < offset; i++)
        {
            pos = pos.Next ?? ints.First;
        }
    }
    else
    {
        long offset = -(value.Value % ints.Count);
        // offset = -value.Value;
        for (long i = 0; i < offset; i++)
        {
            pos = pos.Previous ?? ints.Last;
        }
    }


    if (pos == start)
        return;

    ints.AddAfter(pos, value);
}

(long first, long second, long third) First(List<long> longs, LinkedList<Thing> linkedList)
{
    Thing zeroThing = new(order.IndexOf(0), 0);
    LinkedListNode<Thing>? zeroPos = linkedList.Find(zeroThing);

    LinkedListNode<Thing>? p = zeroPos;
    int thousand = 1000 % longs.Count;
    for (int i = 0; i < thousand; i++)
    {
        p = p.Next ?? p.List.First;
    }

    long first = p.Value.Value;

    p = zeroPos;
    int twoThousand = 2000 % longs.Count;
    for (int i = 0; i < twoThousand; i++)
    {
        p = p.Next ?? p.List.First;
    }

    long second = p.Value.Value;


    p = zeroPos;
    int threeThousand = 3000 % longs.Count;
    for (int i = 0; i < threeThousand; i++)
    {
        p = p.Next ?? p.List.First;
    }

    long third = p.Value.Value;

    return (first, second, third);
}

internal record Input(string Path);

internal record Thing(int Index, long Value);