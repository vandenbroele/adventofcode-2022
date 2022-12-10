using IEnumerator<string> commandEnumerator = File.ReadLines("./input.txt").GetEnumerator();

// test should be 13140

// using IEnumerator<string> commandEnumerator = File.ReadLines("./testInput.txt").GetEnumerator();

int cycle = 0;
int register = 1;

int score = 0;
int temp = 0;

bool hasNext = commandEnumerator.MoveNext();

List<char> display = new(20 * 6);

while (hasNext)
{
    string command = commandEnumerator.Current;
    string[] commands = command.Split(' ');

    cycle++;

    // Part 1
    if ((cycle - 20) % 40 == 0)
    {
        int cycleScore = cycle * register;

        score += cycleScore;
        // Console.WriteLine($"at cycle {cycle} X is {register} resulting in {cycleScore} (total: {score})");
    }

    // Part 2
    int rowPos = (cycle - 1) % 40;

    bool overlapsSprite = rowPos >= register-1 && rowPos <= register + 1;

    display.Add(overlapsSprite ? '#' : '.');


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

Console.WriteLine(score);

// Write display
for (int i = 0; i < display.Count; i++)
{
    if (i % 40 == 0)
    {
        Console.WriteLine();
    }

    Console.Write(display[i]);
}

// output
// ###..####.#..#.#....###...##..#..#..##..
// #..#....#.#..#.#....#..#.#..#.#..#.#..#.
// #..#...#..#..#.#....###..#..#.#..#.#..#.
// ###...#...#..#.#....#..#.####.#..#.####.
// #....#....#..#.#....#..#.#..#.#..#.#..#.
// #....####..##..####.###..#..#..##..#..#.
