string[] inputs = { "./input.txt", "./testInput.txt" };
List<string> lines = File.ReadLines(inputs[0]).ToList();

int result = lines
    .Chunk(3)
    .Select((b, index) => CalculateScore(b[0], b[1], index + 1))
    .Sum();

Console.WriteLine($"part 1: {result}");

const string dividerPacket1 = "[[2]]";
const string dividerPacket2 = "[[6]]";

List<string> sorted = lines
    .Where(l => !string.IsNullOrEmpty(l))
    .Concat(new[] { dividerPacket1, dividerPacket2 })
    .ToList();

sorted.Sort(Compare);

int index1 = sorted.IndexOf(dividerPacket1) + 1;
int index2 = sorted.IndexOf(dividerPacket2) + 1;

Console.WriteLine($"part 2: {index1 * index2}");

int CalculateScore(string left, string right, int index)
{
    bool rightOrder = Compare(left, right) < 0;

    // if (rightOrder)
    // {
    //     Console.WriteLine("=> right order");
    // }
    // else
    // {
    //     Console.WriteLine("=> not right order");
    // }

    return rightOrder ? index : 0;
}

int Compare(string left, string right)
{
    // Console.WriteLine($"Comparing {left} vs {right}");
    bool leftIsArray = left.StartsWith('[');
    bool rightIsArray = right.StartsWith('[');


    if (leftIsArray && rightIsArray)
    {
        List<string> leftParts = GetParts(left).ToList();
        List<string> rightParts = GetParts(right).ToList();

        int minCount = Math.Min(leftParts.Count, rightParts.Count);
        for (int i = 0; i < minCount; i++)
        {
            int compare = Compare(leftParts[i], rightParts[i]);

            if (compare != 0)
            {
                return compare;
            }
        }

        return leftParts.Count - rightParts.Count;
    }

    else if (!leftIsArray && !rightIsArray)
    {
        int comparison = int.Parse(left).CompareTo(int.Parse(right));

        if (comparison != 0)
        {
            return comparison;
        }
    }
    else
    {
        if (!leftIsArray)
        {
            return Compare($"[{left}]", right);
        }
        else
        {
            return Compare(left, $"[{right}]");
        }
    }

    return 0;
}

IEnumerable<string> GetParts(string input)
{
    if (input.StartsWith('['))
    {
        input = input.Substring(1, input.Length - 2);
    }

    int start = 0;
    int braceCount = 0;
    for (int i = 0; i < input.Length; i++)
    {
        char current = input[i];

        switch (current)
        {
            case '[':
                ++braceCount;
                break;
            case ']':
                --braceCount;
                break;
        }

        if (braceCount == 0 && current == ',')
        {
            yield return input.Substring(start, i - start);

            start = i + 1;
        }
    }

    int remainderLength = input.Length - start;
    if (remainderLength > 0)
    {
        yield return input.Substring(start, remainderLength);
    }
}