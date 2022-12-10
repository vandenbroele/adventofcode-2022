using IEnumerator<string> commandEnumerator = File.ReadLines("./input.txt").GetEnumerator();

int cycle = 0;
int register = 1;

int score = 0;
int temp = 0;

bool hasNext = commandEnumerator.MoveNext();

List<char> display = new(20 * 6);

while (hasNext)
{
    string command = commandEnumerator.Current;

    cycle++;

    // Part 1
    if ((cycle - 20) % 40 == 0)
    {
        int cycleScore = cycle * register;
        score += cycleScore;
    }

    // Part 2
    int rowPos = (cycle - 1) % 40;
    bool overlapsSprite = rowPos >= register - 1 && rowPos <= register + 1;

    display.Add(overlapsSprite ? '#' : '.');

    string[] commands = command.Split(' ');
    switch (commands[0])
    {
        case "noop":
            hasNext = commandEnumerator.MoveNext();
            break;
        case "addx":
            temp++;

            if (temp == 2)
            {
                register += int.Parse(commands[1]);
                hasNext = commandEnumerator.MoveNext();
                temp = 0;
            }

            break;
    }
}

Console.WriteLine($"part 1: {score}");
Console.WriteLine("part 2:");
RenderDisplay(display);

void RenderDisplay(IReadOnlyList<char> chars)
{
    for (int i = 0; i < chars.Count; i++)
    {
        if (i % 40 == 0)
        {
            Console.WriteLine();
        }

        Console.Write(chars[i]);
    }
}