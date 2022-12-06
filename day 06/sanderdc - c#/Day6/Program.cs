string input = File.ReadAllText("./input.txt");


int indexPart1 = FindMarkerIndex(input, 4);
Console.WriteLine($"part 1: {indexPart1}");

int indexPart2 = FindMarkerIndex(input, 14);
Console.WriteLine($"part 2: {indexPart2}");

int FindMarkerIndex(string s, int markerLength)
{
    HashSet<char> buffer = new(markerLength);

    int currentIndex = markerLength - 1;
    for (; currentIndex < s.Length; currentIndex++)
    {
        buffer.Clear();

        for (int i = currentIndex - markerLength + 1; i <= currentIndex; i++)
        {
            buffer.Add(s[i]);
        }

        if (buffer.Count == markerLength)
        {
            Console.WriteLine(string.Join("", buffer));
            break;
        }
    }

    return currentIndex + 1;
}