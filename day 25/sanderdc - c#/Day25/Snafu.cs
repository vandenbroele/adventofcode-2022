using System.Numerics;

public readonly struct Snafu
{
    private BigInteger Value { get; }

    private Snafu(long value)
    {
        Value = value;
    }

    private Snafu(BigInteger value)
    {
        Value = value;
    }

    public int ToInt() => (int)Value;
    public long ToLong() => (long)Value;
    public BigInteger ToBigInt() => Value;

    public override string ToString()
    {
        List<char> stuff = new();

        BigInteger remainder = Value;
        int place = 0;

        while (remainder > 0)
        {
            BigInteger mul = BigInteger.Pow(5, place);
            BigInteger nextMul = BigInteger.Pow(5, place + 1);
            BigInteger value = remainder % nextMul;

            int localValue = (int)(value / mul);

            remainder -= value;

            if (localValue > 2)
            {
                const int carry = 1;
                remainder += nextMul * carry;

                stuff.Insert(0, (5 - (int)localValue) switch
                {
                    1 => '-',
                    2 => '=',
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
            else
            {
                stuff.Insert(0, (localValue).ToString()[0]);
            }


            ++place;
        }

        return new string(stuff.ToArray());
    }

    public static Snafu Parse(string input)
    {
        BigInteger value = 0;

        for (int place = 0; place < input.Length; place++)
        {
            BigInteger mul = BigInteger.Pow(5, place);
            int localValue = input[^(place + 1)] switch
            {
                '2' => 2,
                '1' => 1,
                '0' => 0,
                '-' => -1,
                '=' => -2,
                _ => throw new ArgumentOutOfRangeException()
            };

            value += mul * localValue;
        }

        return new Snafu(value);
    }

    public static Snafu FromInt(int input) => new(input);
    public static Snafu FromLong(long input) => new(input);
    public static Snafu FromBigInt(BigInteger input) => new(input);
}