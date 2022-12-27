using System.Numerics;

namespace Day25.Tests;

public class SnafuTests
{
    [Test]
    [TestCase(1, "1")]
    [TestCase(10, "20")]
    [TestCase(31, "111")]
    [TestCase(8, "2=")]
    public void ToString_Validate(int value, string expected)
    {
        Snafu snafu = Snafu.FromInt(value);
        Assert.That(snafu.ToString(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("1", 1)]
    [TestCase("20", 10)]
    [TestCase("2=", 8)]
    public void Parse_Validate(string input, int expected)
    {
        Snafu snafu = Snafu.Parse(input);

        Assert.That(snafu.ToInt(), Is.EqualTo(expected));
    }

    [TestCase(1, "1")]
    [TestCase(2, "2")]
    [TestCase(3, "1=")]
    [TestCase(4, "1-")]
    [TestCase(5, "10")]
    [TestCase(6, "11")]
    [TestCase(7, "12")]
    [TestCase(8, "2=")]
    [TestCase(9, "2-")]
    [TestCase(10, "20")]
    [TestCase(15, "1=0")]
    [TestCase(20, "1-0")]
    [TestCase(2022, "1=11-2")]
    [TestCase(12345, "1-0---0")]
    [TestCase(314159265, "1121-1110-1=0")]
    public void ToString_ValidateChart(int value, string expected)
    {
        Snafu snafu = Snafu.FromInt(value);

        Assert.That(snafu.ToString(), Is.EqualTo(expected));
    }

    [TestCase("1", 1)]
    [TestCase("2", 2)]
    [TestCase("1=", 3)]
    [TestCase("1-", 4)]
    [TestCase("10", 5)]
    [TestCase("11", 6)]
    [TestCase("12", 7)]
    [TestCase("2=", 8)]
    [TestCase("2-", 9)]
    [TestCase("20", 10)]
    [TestCase("1=0", 15)]
    [TestCase("1-0", 20)]
    [TestCase("1=11-2", 2022)]
    [TestCase("1-0---0", 12345)]
    [TestCase("1121-1110-1=0", 314159265)]
    public void Parse_ValidateChart(string value, int expected)
    {
        Snafu snafu = Snafu.Parse(value);

        Assert.That(snafu.ToInt(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("1-1-211011=22122-=2-", "15937555647959")]
    public void Parse_Big(string value, string expectedNumber)
    {
        BigInteger expected = BigInteger.Parse(expectedNumber);
        Snafu snafu = Snafu.Parse(value);

        Assert.That(snafu.ToBigInt(), Is.EqualTo(expected));
    }
    
    [Test]
    [TestCase("15937555647959", "1-1-211011=22122-=2-")]
    public void ToString_Big(string input, string expected)
    {
        Snafu snafu = Snafu.FromBigInt(BigInteger.Parse(input));

        Assert.That(snafu.ToString(), Is.EqualTo(expected));
    }
    
}