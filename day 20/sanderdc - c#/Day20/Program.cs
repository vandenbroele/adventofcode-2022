Input[] inputs =
{
    new("./testInput.txt"),
    new("./input.txt")
};

List<string> inputLines = File.ReadLines(inputs[1].Path).ToList();

List<int> order = inputLines.Select(int.Parse).ToList();
LinkedList<Thing> sequence = new(order
    .Select((value, index) => new Thing(index, value)));

for (int index = 0; index < order.Count; index++)
{
    int current = order[index];
    Move(sequence, new Thing(index, current));
    // Console.WriteLine(string.Join(", ", sequence));
}

// Console.WriteLine(string.Join(", ", sequence));

Thing zeroThing = new(order.IndexOf(0), 0);
LinkedListNode<Thing>? zeroPos = sequence.Find(zeroThing);


LinkedListNode<Thing>? p = zeroPos;
int thousand = 1000 % order.Count;
for (int i = 0; i < thousand; i++)
{
    p = p.Next ?? p.List.First;
}

int first = p.Value.Value;

p = zeroPos;
int twoThousand = 2000 % order.Count;
for (int i = 0; i < twoThousand; i++)
{
    p = p.Next ?? p.List.First;
}

int second = p.Value.Value;


p = zeroPos;
int threeThousand = 3000 % order.Count;
for (int i = 0; i < threeThousand; i++)
{
    p = p.Next ?? p.List.First;
}

int third = p.Value.Value;

Console.WriteLine($"part 1: {first + second + third}");

void Move(LinkedList<Thing> ints, Thing value)
{
    LinkedListNode<Thing>? start = ints.Find(value)!;

    var pos = start.Previous ?? ints.Last;
    ints.Remove(start);

    if (value.Value > 0)
    {
        int offset = value.Value % ints.Count;
        for (int i = 0; i < offset; i++)
        {
            pos = pos.Next ?? ints.First;
        }
    }
    else
    {
        int offset = -(value.Value % ints.Count);
        for (int i = 0; i < offset; i++)
        {
            pos = pos.Previous ?? ints.Last;
        }
    }


    if (pos == start)
        return;

    ints.AddAfter(pos, value);
}

internal record Input(string Path);

internal record Thing(int Index, int Value);