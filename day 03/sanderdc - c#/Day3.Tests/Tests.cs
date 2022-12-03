namespace Day3.Tests;

public class Tests
{
    [Test]
    [TestCase('a', 1)]
    [TestCase('z', 26)]
    [TestCase('A', 27)]
    [TestCase('Z', 52)]
    public void Priority_Validate(char input, int expectedScore)
    {
        Assert.AreEqual(expectedScore, Helpers.GetCharPriority(input));
    }

    [Test]
    [TestCase("vJrwpWtwJgWrhcsFMMfFFhFp", 'p')]
    [TestCase("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", 'L')]
    [TestCase("PmmdzqPrVvPwwTWBwg", 'P')]
    [TestCase("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", 'v')]
    [TestCase("ttgJtRGJQctTZtZT", 't')]
    [TestCase("CrZsJsPPZsGzwwsLwLmpwMDw", 's')]
    public void Overlap_Validate(string input, char expected)
    {
        Assert.AreEqual(expected, Helpers.GetOverlap(input));
    }

    [Test]
    [TestCase(
        "JrwpWtwJgWrhcsFMMfFFhFp",
        "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL",
        "PmmdzqPrVvPwwTWBwg",
        'r')]
    [TestCase(
        "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn",
        "ttgJtRGJQctTZtZT",
        "CrZsJsPPZsGzwwsLwLmpwMDw",
        'Z')]
    public void Overlap_Validate(string input1, string input2, string input3, char expected)
    {
        Assert.AreEqual(expected, Helpers.GetOverlap(input1, input2, input3));
    }

    [Test]
    [TestCase(new[] { "", "", "" }, 1)]
    [TestCase(new[] { "", "", "", "", "", "" }, 2)]
    public void Chunk_Validate(IEnumerable<string> input, int expectedChunks)
    {
        Assert.AreEqual(expectedChunks, input.Chunk().Count());
    }
}