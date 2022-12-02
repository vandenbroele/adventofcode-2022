namespace Day2.Tests;

public class ScorePart2Tests
{
    [Test]
    [TestCase('A', 'Y', 'A')]
    [TestCase('B', 'X', 'A')]
    [TestCase('C', 'Z', 'A')]
    public void GetHand_Validation(char other, char input, char expectedValue)
    {
        Assert.AreEqual(expectedValue, ScorePart2.GetHand(other, input));
    }
}