List<string> lines = File.ReadLines("./input.txt").ToList();

Stacks stacks = new(9);

List<string> setup = lines.Take(8).ToList();
InitialRead(setup);

void InitialRead(IReadOnlyList<string> list)
{
    for (int i = list.Count - 1; i >= 0; i--)
    {
        string line = list[i];
        int counter = 0;
        for (int j = 1; j < line.Length; j += 4)
        {
            char c = line[j];
            if (char.IsLetter(c))
            {
                stacks.Add(counter, c);
            }

            ++counter;
        }
    }
}

IEnumerable<string> moves = lines.Skip(10);

foreach (string move in moves)
{
    // Console.WriteLine(move);

    string[] splitInput = move.Split(' ');
    int count = int.Parse(splitInput[1]);
    int from = int.Parse(splitInput[3]);
    int to = int.Parse(splitInput[5]);

    // for (int i = 0; i < count; i++)
    // {
    //     stacks.Move(from - 1, to - 1);
    // }
    stacks.MoveStack(from-1, to-1, count);
}

Console.WriteLine($"part 2: {string.Join("", stacks.GetTops())}");



internal class Stacks
{
    private readonly List<Stack<char>> stacks;

    public Stacks(int count)
    {
        stacks = new List<Stack<char>>(count);
        for (int i = 0; i < count; i++)
        {
            stacks.Add(new Stack<char>());
        }
    }

    public void Add(int index, char c)
    {
        stacks[index].Push(c);
    }

    public void Move(int from, int to)
    {
        stacks[to].Push(stacks[from].Pop());
    }

    public void MoveStack(int from, int to, int count)
    {
        List<char> temp = new(count);

        for (int i = 0; i < count; i++)
        {
            temp.Add(stacks[from].Pop());
        }

        for (int i = temp.Count - 1; i >= 0; i--)
        {
            stacks[to].Push(temp[i]);
        }
    }

    public IEnumerable<char> GetTops() => stacks.Select(stack => stack.Peek());
}