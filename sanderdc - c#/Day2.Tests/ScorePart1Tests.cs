namespace Day2.Tests;

public class ScorePart1Tests
{
    [Test]
    [TestCase('A', 1)]
    [TestCase('B', 2)]
    [TestCase('C', 3)]
    [TestCase('X', 1)]
    [TestCase('Y', 2)]
    [TestCase('Z', 3)]
    public void CalculateHand_Validation(char hand, int expectedValue)
    {
        Assert.AreEqual(expectedValue, ScorePart1.CalculateHand(hand));
    }

    [Test]
    [TestCase('A', 'X', 3)]
    [TestCase('B', 'Y', 3)]
    [TestCase('C', 'Z', 3)]
    public void CalculateRoundOutcome_DrawValidation(char hand1, char hand2, int expectedValue)
    {
        Assert.AreEqual(expectedValue, ScorePart1.CalculateRoundOutcome(hand1, hand2));
    }

    [Test]
    [TestCase('A', 'Y', 6)]
    [TestCase('B', 'Z', 6)]
    [TestCase('C', 'X', 6)]
    public void CalculateRoundOutcome_WinValidation(char hand1, char hand2, int expectedValue)
    {
        Assert.AreEqual(expectedValue, ScorePart1.CalculateRoundOutcome(hand1, hand2));
    }

    [Test]
    [TestCase('A', 'Z', 0)]
    [TestCase('B', 'X', 0)]
    [TestCase('C', 'Y', 0)]
    public void CalculateRoundOutcome_LoseValidation(char hand1, char hand2, int expectedValue)
    {
        Assert.AreEqual(expectedValue, ScorePart1.CalculateRoundOutcome(hand1, hand2));
    }

    [Test]
    [TestCase('A', 'Y', 8)]
    [TestCase('B', 'X', 1)]
    [TestCase('C', 'Z', 6)]
    // Validate self
    [TestCase('A', 'B', 8)]
    [TestCase('B', 'A', 1)]
    [TestCase('C', 'C', 6)]
    public void CalculateRound_Validation(char hand1, char hand2, int expectedValue)
    {
        Assert.AreEqual(expectedValue, ScorePart1.CalculateRound(hand1, hand2));
    }
}