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
    [TestCase(new[]
        {
            "JrwpWtwJgWrhcsFMMfFFhFp",
            "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL",
            "PmmdzqPrVvPwwTWBwg"
        },
        'r')]
    [TestCase(new[]
        {
            "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn",
            "ttgJtRGJQctTZtZT",
            "CrZsJsPPZsGzwwsLwLmpwMDw"
        },
        'Z')]
    public void Overlap_Validate(string[] input, char expected)
    {
        Assert.AreEqual(expected, Helpers.GetOverlap(input));
    }
}